using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using AutoMapper;
using Berico.SnagL.Host.Facebook.Models;
using Facebook;
using Facebook.Web.Mvc;
using Newtonsoft.Json;
using SnagL.FacebookHostDemo.GraphML;

namespace Berico.SnagL.Host.Facebook.Controllers
{
    public class UserInformationController : Controller
    {
        //
        // GET: /UserInformation/
        [FacebookAuthorize]
        public ActionResult Index()
        {
            string xml;
            FacebookApp app = new FacebookApp();
            JsonObject me = (JsonObject)app.Get("me");
            JsonObject friends = (JsonObject)app.Get("me/friends");
            FacebookFriends facebookFriends = JsonConvert.DeserializeObject<FacebookFriends>(friends.ToString());
            XmlSerializer serializer = new XmlSerializer(typeof(GraphML));
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            List<FacebookUser> users = new List<FacebookUser>();
            GraphML graphML = new GraphML();

            // Add Users
            users.Add(JsonConvert.DeserializeObject<FacebookUser>(me.ToString()));

            foreach (FacebookUser user in facebookFriends.Friends)
            {
                JsonObject friend = (JsonObject)app.Get(user.Id.ToString());
                users.Add(JsonConvert.DeserializeObject<FacebookUser>(friend.ToString()));
            }

            // Prepare the keys
            AddKeys(graphML.Keys);

            // Add the Berico namspace
            namespaces.Add("berico", "http://graph.bericotechnologies.com/xmlns");

            foreach (FacebookUser user in users)
            {
                graphML.Graph.Nodes.Add(Mapper.Map<FacebookUser, Node>(user));
            }

            using (MemoryStream stream = new MemoryStream())
            {
                XmlDocument doc = new XmlDocument();

                serializer.Serialize(stream, graphML, namespaces);
                stream.Seek(0, SeekOrigin.Begin);
                doc.Load(stream);

                // Read the XML into a string object
                xml = doc.OuterXml;
            }

            return this.Content(xml, "txt/xml");
        }

        /// <summary>
        /// Prepares the key collection
        /// </summary>
        /// <param name="keys">Collection that will store keys</param>
        private void AddKeys(Collection<Key> keys)
        {
            // TODO: The strings in this method should be made constants in a public class
            List<string> attributes = new List<string>
            {
                "Name",
                "FirstName",
                "LastName",
                "Birthday",
                "Hometown"
            };

            List<string> requiredAttributes = new List<string>
            {
                "Description",
                "DisplayValue",
                "ImageSource",
                "Position",
                "IsHidden",
                "BackgroundColor",
                "SelectionColor"
            };

            foreach (string attribute in attributes)
            {
                keys.Add(new Key
                {
                    KeyId = String.Format("node-attr-{0}-desc", attribute),
                    Scope = "node",
                    Target = String.Format("node-attr-{0}-desc", attribute),
                    DataType = "string"
                });

                keys.Add(new Key
                {
                    KeyId = String.Format("node-attr-{0}-val", attribute),
                    Scope = "node",
                    Target = String.Format("node-attr-{0}-val", attribute),
                    DataType = "string"
                });
            }

            foreach (string attribute in requiredAttributes)
            {
                keys.Add(new Key
                {
                    KeyId = String.Format("node-prop-{0}", attribute),
                    Scope = "node",
                    Target = String.Format("node-prop-{0}", attribute),
                    DataType = "string"
                });
            }
        }
    }
}
