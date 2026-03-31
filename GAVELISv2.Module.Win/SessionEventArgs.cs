using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
namespace GAVELISv2.Module.Win {
    public class SessionEventArgs : EventArgs {
        private UnitOfWork session;
        public SessionEventArgs(UnitOfWork session) { this.session = session; }
        public UnitOfWork Session { get { return session; } }
    }
}
