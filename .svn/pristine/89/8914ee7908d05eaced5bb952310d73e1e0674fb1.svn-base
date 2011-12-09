//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Resources;
using System.Xml;
using Berico.SnagL.Infrastructure.Configuration;

namespace Berico.SnagL.Infrastructure
{
    /// <summary>
    /// Provides functionality for lazy-loading XAP files
    /// </summary>
    public class XapLoader
    {
        #region Fields

        /// <summary>
        /// Stores the web clients to access the URL of the requested XAPs.
        /// This is used because of issues using refelection to access the
        /// FinalUri property of the Stream reference in the OpenReadCompleted
        /// event handlers.
        /// </summary>
        private IDictionary<WebClient, string> _assemblies = new Dictionary<WebClient, string>();

        /// <summary>
        /// Stores the web clients to access the types for the requested XAPs.
        /// This is used because of issues using refelection to access the
        /// FinalUri property of the Stream reference in the OpenReadCompleted
        /// event handlers.
        /// </summary>
        private IDictionary<WebClient, string> _typeNames = new Dictionary<WebClient, string>();

        #endregion

        #region Events

        /// <summary>
        /// Raised when the XAP loader has finished loading XAP file
        /// </summary>
        public event EventHandler<XapLoadedEventArgs> XapLoaded;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the XapLoader class
        /// </summary>
        public XapLoader()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Loads any number of external XAP resources from a collection. Raises the XapLoaded after all URIs have been processed
        /// </summary>
        /// <param name="externalResources">A collection of external resources to load</param>
        public void BeginLoadXap(ExternalResource[] externalResources)
        {
            BeginLoadXap(externalResources, true);
        }

        /// <summary>
        /// Loads any number of external XAP resources from a collection
        /// </summary>
        /// <param name="externalResources">A collection of external resources to load</param>
        /// <param name="raiseEventOnlyAtEnd">Wait until all XAP URIs have loaded before raising the XapLoaded event</param>
        public void BeginLoadXap(ExternalResource[] externalResources, bool raiseEventOnlyAtEnd)
        {
            WebClient client;

            for (int x = 0; x < externalResources.Length; x++)
            {
                client = new WebClient();

                if (raiseEventOnlyAtEnd)
                {
                    if (!(x + 1 < externalResources.Length))
                    {
                        client.OpenReadCompleted += OpenReadCompletedRaiseEvent;
                    }
                    else
                    {
                        client.OpenReadCompleted += OpenReadCompletedNoEvent;
                    }
                }
                else
                {
                    client.OpenReadCompleted += OpenReadCompletedRaiseEvent;
                }

                FetchXap(externalResources[x], client);
            }
        }

        /// <summary>
        /// Asyncronously loads the XAP file at the specified URI
        /// </summary>
        /// <param name="resource">URI to the XAP file for which to load</param>
        /// <param name="typeName">Specifies the type name to load</param>
        public void BeginLoadXap(ExternalResource resource)
        {
            WebClient client = new WebClient();

            // Wire up the OpenReadCompleted event handler
            client.OpenReadCompleted += OpenReadCompletedRaiseEvent;

            FetchXap(resource, client);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads the specified resource
        /// </summary>
        /// <param name="resource">The external resource to load</param>
        /// <param name="client">The WebClient object used to fetch the XAP</param>
        private void FetchXap(ExternalResource resource, WebClient client)
        {
            _assemblies.Add(client, Path.GetFileNameWithoutExtension(resource.Path));
            _typeNames.Add(client, resource.Assembly);

            client.OpenReadAsync(new Uri(resource.Path, UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// Loads an external assmebly from the specified stream
        /// </summary>
        /// <param name="stream">Stream from which to load the assembly</param>
        private object LoadExternalAssembly(WebClient client, Stream stream)
        {
            StreamResourceInfo manifestStream = Application.GetResourceStream(new StreamResourceInfo(stream, null), new Uri("AppManifest.xaml", UriKind.Relative));
            string appManifest = new StreamReader(manifestStream.Stream).ReadToEnd();
            string assemblyName = _assemblies[client] + ".dll";
            string typeName = _typeNames[client];
            XmlReader reader = XmlReader.Create(new StringReader(appManifest));
            Assembly asm = null;
            object instance = null;

            while (reader.Read())
            {
                if (reader.IsStartElement("AssemblyPart"))
                {
                    reader.MoveToAttribute("Source");
                    reader.ReadAttributeValue();

                    if (reader.Value == assemblyName)
                    {
                        var assemblyStream = new StreamResourceInfo(stream, "application/binary");
                        var resourceStream = Application.GetResourceStream(assemblyStream, new Uri(reader.Value, UriKind.Relative));
                        AssemblyPart p = new AssemblyPart();
                        asm = p.Load(resourceStream.Stream);

                        break;
                    }
                }
            }

            if (asm == null)
            {
                throw new InvalidOperationException("Could not find specified assembly.");
            }

            // Instantiate an instance of the specified type
            if (!String.IsNullOrWhiteSpace(typeName))
            {
                instance = asm.CreateInstance(typeName);

                if (instance == null)
                {
                    throw new InvalidOperationException("Could not create instance of requested type.");
                }
            }

            return instance;
        }

        /// <summary>
        /// Runs when the web client has finished downloading the XAP file without raising XapLoaded event
        /// </summary>
        /// <param name="sender">The WebClient object</param>
        /// <param name="e">Any event arguments passed by the WebClient</param>
        private void OpenReadCompletedNoEvent(object sender, OpenReadCompletedEventArgs e)
        {
            LoadExternalAssembly((sender as WebClient), e.Result);
        }

        /// <summary>
        /// Runs when the web client has finished downloading the XAP file and raises the XapLoaded event
        /// </summary>
        /// <param name="sender">The WebClient object</param>
        /// <param name="e">Any event arguments passed by the WebClient</param>
        private void OpenReadCompletedRaiseEvent(object sender, OpenReadCompletedEventArgs e)
        {
            object instance = LoadExternalAssembly((sender as WebClient), e.Result);

            // Raise the XapLoaded event
            if (XapLoaded != null)
            {
                //XapLoaded(this, new XapLoadedEventArgs(instance));
                XapLoaded(this, new XapLoadedEventArgs(instance));
            }
        }

        #endregion
    }
}
