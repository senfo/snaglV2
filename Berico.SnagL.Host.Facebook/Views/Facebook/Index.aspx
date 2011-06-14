<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript" src="scripts/Silverlight.js"></script>
    <script type="text/javascript">

        var graph = null;                           // Represents the scriptable object within Snagl
        var preload = true;                         // Indicates whether sample data should be preloaded
        var sampleDataFile = "/UserInformation";   // Indicates the name of the file containg the sample data to be preloaded

        // This event is fired after the Silverlight plugin (Snagl)
        // is fully loaded.  The "sender" is a reference to the actual
        // plugin so we save it into a variable for safe keeping.  All
        // initiall calls to Snagl should be place within this function.
        function onSnaglRootLoaded(sender, args) {
            // Get the instance of the scriptable object
            // within the SnagL application that is used
            // to interact with SnagL
            graph = sender.getHost().Content.SNAGL;

            // Wire up any events
            var snaglLoadedToken = snagl.addEventListener("SnaglLoaded", onSnaglLoaded);
        }

        //
        function onSnaglLoaded(sender, args) {

            // Preload data into SnagL, if configured to do so
            if (preload)
                preLoadGraphdata(preLoadCompleteCallback, sampleDataFile);

            snagl.LoadGraphML(val);
        }

        // This function loads the provided text file (as text) from the
        // server and returns its contents to the method that called 
        function preLoadGraphdata(callback, dataFile) {

            // This call uses an AJAX call, back to the server, in order
            // to synchronously get the desired file.  The contents of
            // the file, once retrieved, is passed to the specified
            // callback function.
            $.ajax({
                type: "GET",
                url: dataFile,
                dataType: "text",
                async: false,
                success: callback
            });

        }

        //
        function preLoadCompleteCallback(data) {

            // This call loads the provided data into the graph.  The 
            // 'LoadGraphML' method is capable of parsing
            // GaphML data.
            snagl.LoadGraphML(data);

        }

        function onSilverlightError(sender, args) {
            var appSource = "";
            if (sender != null && sender != 0) {
                appSource = sender.getHost().Source;
            }

            var errorType = args.ErrorType;
            var iErrorCode = args.ErrorCode;

            if (errorType == "ImageError" || errorType == "MediaError") {
                return;
            }

            var errMsg = "Unhandled Error in Silverlight Application " + appSource + "\n";

            errMsg += "Code: " + iErrorCode + "    \n";
            errMsg += "Category: " + errorType + "       \n";
            errMsg += "Message: " + args.ErrorMessage + "     \n";

            if (errorType == "ParserError") {
                errMsg += "File: " + args.xamlFile + "     \n";
                errMsg += "Line: " + args.lineNumber + "     \n";
                errMsg += "Position: " + args.charPosition + "     \n";
            }
            else if (errorType == "RuntimeError") {
                if (args.lineNumber != 0) {
                    errMsg += "Line: " + args.lineNumber + "     \n";
                    errMsg += "Position: " + args.charPosition + "     \n";
                }
                errMsg += "MethodName: " + args.methodName + "     \n";
            }

            throw new Error(errMsg);
        }

    </script>
    <div id="silverlightControlHost">
        <script type="text/javascript">
            var snagl = null;

            function onSnaglRootLoaded(sender) {
                snagl = sender.getHost().Content.SNAGL;
                var entertoken1 = snagl.addEventListener("SnaglLoaded", onSnaglLoaded);
            }

            function onSnaglLoaded(args) {
                $.ajax({
                    type: "GET",
                    url: sampleDataFile,
                    dataType: "text",
                    async: false,
                    success: function (data) {
                        val = data;
                    }
                });

                snagl.LoadGraphML(val);
            }

        </script>
        <object id="snagl_silverlight" data="data:application/x-silverlight-2," type="application/x-silverlight-2"
            width="100%" height="100%">
            <param name="enableFramerateCounter" value="true" />
            <param name="maxFrameRate" value="25" />
            <param name="enableGPUAcceleration" value="true" />
            <param name="source" value="ClientBin/Berico.SnagL.xap" />
            <param name="onError" value="onSilverlightError" />
            <param name="background" value="white" />
            <param name="minRuntimeVersion" value="4.0.50826.0" />
            <param name="autoUpgrade" value="true" />
            <param name="onLoad" value="onSnaglRootLoaded" />
            <param name="initParams" value="loggerLevel=DEBUG,theme=Resources/default.xaml,loggerProvider=Logger.Provider.IsolatedStorage,preferencesProvider=Preference.Provider.IsolatedStorage" />
            <a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=4.0.50401.0" style="text-decoration: none">
                <img src="http://go.microsoft.com/fwlink/?LinkId=161376" alt="Get Microsoft Silverlight"
                    style="border-style: none" />
            </a>
        </object>
        <iframe id="_sl_historyFrame" style="visibility: hidden; height: 0px; width: 0px; border: 0px"></iframe>
    </div>
</asp:Content>
