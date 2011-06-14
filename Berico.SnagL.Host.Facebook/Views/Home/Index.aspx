<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Home Page
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <p>
        <img id="fbLogin" src="../../Content/login-button.png" alt="Facebook Login Button" />
    </p>
    <div id="fb-root">
    </div>
    <script src="http://connect.facebook.net/en_US/all.js" type="text/javascript"></script>
    <script type="text/javascript">
        FB.init({ appId: '<%:FacebookSettings.Current.AppId %>', status: true, cookie: true, xfbml: true });
        $('#fbLogin').click(function () {
            FB.login(function (response) {
                if (response.session) {
                    window.location = '<%:Url.Action("Index", "Facebook") %>'
                } else {
                    // user cancelled login
                }
            }, { perms: "publish_stream, read_stream, user_likes, friends_likes, friends_hometown" });
        });
    </script>
</asp:Content>
