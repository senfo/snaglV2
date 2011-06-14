//-------------------------------------------------------------
// Copyright © Berico Technologies, LLC. All Rights Reserved
//
// This source is subject to the Microsoft Public License. Please
// visit http://www.microsoft.com/opensource/licenses.mspx#Ms-PL
// for more information.
//
// SnagL™ is a trademark of Berico Technologies.
//-------------------------------------------------------------

namespace Berico.Common.Events
{
    public class DetailedEventInformation
    {
        public object Sender { get; set; }
        public object EventArgs { get; set; }
        public object CommandArgument { get; set; }
    }
}