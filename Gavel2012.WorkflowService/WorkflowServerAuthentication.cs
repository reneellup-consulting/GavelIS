using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.ExpressApp.Security;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;

namespace Gavel2012.WorkflowService
{
    public class WorkflowServerAuthentication : AuthenticationBase
    {
        private CriteriaOperator workflowWorkerUserCriteria;
        public WorkflowServerAuthentication(CriteriaOperator workflowWorkerUserCriteria)
        {
            this.workflowWorkerUserCriteria = workflowWorkerUserCriteria;
        }
        public override object Authenticate(IObjectSpace objectSpace)
        {
            object user = objectSpace.FindObject(UserType, workflowWorkerUserCriteria);
            if (user == null)
            {
                throw new AuthenticationException("", "Cannot find workflow worker user.");
            }
            return user;
        }
        public override Type UserType { get; set; }
        public override bool AskLogonParametersViaUI
        {
            get { return false; }
        }
        public override bool IsLogoffEnabled
        {
            get { return false; }
        }
    }
}
