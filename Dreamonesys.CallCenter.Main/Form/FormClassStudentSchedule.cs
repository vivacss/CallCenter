using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dreamonesys.CallCenter.Main
{
    public partial class FormClassStudentSchedule : Form
    {
        #region Property

        public string StudyType { get; set; }
        public string ClassEmployeeCPNO { get; set; }
        public string ClassEmployeeCLNO { get; set; }
        public string ClassStudentCPNO { get; set; }
        public string ClassStudentUID { get; set; }
        public string ClassEmployeeUID { get; set; }
        public string ClassSchoolCDStudy { get; set; }

        #endregion Property

        #region Constructor

        public FormClassStudentSchedule()
        {
            InitializeComponent();
        }

        public UserControlStudy UserControl { get; set; }

        #endregion Constructor

        #region Event

        private void FormClassStudentSchedule_Load(object sender, EventArgs e)
        {

            UserControlStudy userControl = new UserControlStudy();
            //패널 사용자 컨트롤
            //this.Controls.Add(userControl);            
            //userControl.StudyType = this.StudyType;
            //userControl.ClassEmployeeCPNO = this.ClassEmployeeCPNO;
            //userControl.ClassEmployeeCLNO = this.ClassEmployeeCLNO;
            this.Controls.Add(userControl);         
            userControl.Visible = true;
            userControl.Select(this.StudyType, this.ClassEmployeeCPNO, this.ClassEmployeeCLNO, this.ClassStudentCPNO, this.ClassStudentUID, this.ClassEmployeeUID, this.ClassSchoolCDStudy);

        }

        #endregion Event

    }
}
