using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Dreamonesys.CallCenter.Main.Class;

namespace Dreamonesys.CallCenter.Main
{
    /// <summary>
    /// 콜센터 메인 클래스
    /// </summary>
    public partial class FormMain : Form
    {

        #region Field

        private Common _common;
        private AppMain _appMain;
        private UserControlStudy _userControlStudy;

        #endregion Field

        #region Property
        
        //차시관리 과정2 탭으로 이동 조회
        public string StudyType { get; set; }
        public string ClassEmployeeCPNO { get; set; }
        public string ClassEmployeeCLNO { get; set; }
        public string ClassStudentCPNO { get; set; }
        public string ClassStudentUID { get; set; }
        public string ClassEmployeeUID { get; set; }
        public string ClassSchoolCDStudy { get; set; }
  
        #endregion Property

        #region Constructor

        /// <summary>
        /// 기본 생성자
        /// </summary>
        public FormMain()
        {
            InitializeComponent();

            // 공통 모듈 클래스 인스턴스 생성
            _common = new Common();
            // 프로그램 정보 클래스 인스턴스 생성
            _appMain = new AppMain();
            // 공용 모듈에서 프로그램 정보를 참조할 수 있도록 함
            _common._appMain = _appMain;
            // 프로그램 정보에서 메인 폼을 참조할 수 있도록 함
            _appMain.MainForm = this;
            // 프로그램명 설정
            _appMain.ProgramName = "유투엠 콜센터 1.0";
        }

        

        #endregion Constructor

        #region Method

        /// <summary>
        /// 콤보박스 리스트를 조회한다.
        /// </summary>
        /// <history>
        /// 박석제, 2014-09-24, 생성
        /// </history>
        public void InitCombo()
        {
            // 캠퍼스 구분 콤보박스 데이터 생성
            //_common.GetComboList(comboBoxCampusType, "캠퍼스구분", true);
            // 캠퍼스 콤보박스 데이터 생성
            //_common.GetComboList(comboBoxCampus, "캠퍼스", true);

            // 콤보박스 멀티
            Common.ComboBoxList[] comboBoxList = 
            {
                //main tab 콤보박스
                new Common.ComboBoxList(comboBoxCampusType, "캠퍼스구분", true),
                new Common.ComboBoxList(comboBoxCampus, "캠퍼스", true),              
                //콩알관리 콤보박스
                new Common.ComboBoxList(comboBoxCampusTypePoint, "캠퍼스구분", true),
                new Common.ComboBoxList(comboBoxCampusPoint, "캠퍼스", true) ,  
                new Common.ComboBoxList(comboBoxSchoolCDPoint, "학교급", true), 
                new Common.ComboBoxList(comboBoxPointCode, "콩알코드", true),                 
                //차시관리 콤보박스
                new Common.ComboBoxList(comboBoxCampusTypeStudy, "캠퍼스구분", true),
                new Common.ComboBoxList(comboBoxCampusStudy, "캠퍼스", true) ,  
                new Common.ComboBoxList(comboBoxYyyyStudy, "년도", true) , 
                new Common.ComboBoxList(comboBoxSchoolCDStudy, "학교급", true),
                new Common.ComboBoxList(comboBoxTermCDStudy, "분기", true),
                new Common.ComboBoxList(comboBoxUseYNStudy, "사용", true),
                //미결사용자 등록 콤보박스
                new Common.ComboBoxList(comboBoxCampusTypeByPass, "캠퍼스구분", true),
                new Common.ComboBoxList(comboBoxCampusByPass, "캠퍼스", true)
                
            };
            this._common.GetComboList(comboBoxList);
        }

        /// <summary>
        /// 사용자 정의 목록을 조회한다.
        /// </summary>
        /// <history>
        /// 박석제, 2014-09-24, 생성
        /// 박석제, 2014-10-08, 조회시 check_yn 컬럼 출력 예외 추가
        /// </history>
        private void SelectDataGridView(DataGridView pDataGridView, string pQueryKind)
        {
            SqlCommand sqlCommand = null;
            SqlResult sqlResult = new SqlResult();

            // 그리드 초기화
            switch (pDataGridView.Name)
            {
                case "dataGridViewCampus":
                    dataGridViewClassEmployee.Rows.Clear();
                    dataGridViewEmployee.Rows.Clear();                    
                    break;
                case "dataGridViewEmployee":
                    dataGridViewClassEmployee.Rows.Clear();
                    dataGridViewClassStudent.Rows.Clear();
                    dataGridViewEduStudent.Rows.Clear();
                    break;
                case "dataGridViewClassEmployee":
                    dataGridViewClassStudent.Rows.Clear();
                    dataGridViewEduStudent.Rows.Clear();
                    break;
                case "dataGridViewCampusPoint":
                    dataGridViewClassPoint.Rows.Clear();
                    dataGridViewStudentPoint.Rows.Clear();
                    break;
                case "dataGridViewClassPoint":
                    dataGridViewStudentPoint.Rows.Clear();
                    dataGridViewStudentPointSave.Rows.Clear();
                    break;
                case "dataGridViewStudentPoint":                    
                    dataGridViewStudentPointSave.Rows.Clear();
                    break;
                case "dataGridViewClass":
                    dataGridViewStudent.Rows.Clear();
                    //dataGridViewClassStudy.Rows.Clear();                                  
                    break;
                case "dataGridViewStudent":                                        
                    //dataGridViewStudentStudy.Rows.Clear();                    
                    break;
                case "dataGridViewClassStudy":
                    //dataGridViewClassStudentSchedule.Rows.Clear();                    
                    break;
                case "dataGridViewStudentStudy":
                    //dataGridViewClassStudentSchedule.Rows.Clear();   
                    break;
                case "dataGridViewByPass":
                    dataGridViewByPassEmp.Rows.Clear();
                    dataGridViewByPassUser.Rows.Clear();
                    break;
                
                default:
                    break;
            }

            this.Cursor = Cursors.WaitCursor;

            try
            {
                CreateSql(ref sqlCommand, pQueryKind);

                // 쿼리실행 -> 결과값 저장
                this._common.Execute(sqlCommand, ref sqlResult);

                // 성공여부 판단
                if (sqlResult.Success == true)
                {
                    //그리드 초기화
                    pDataGridView.Rows.Clear();

                    // 데이터 테이블 행 루프
                    foreach (DataRow row in sqlResult.DataTable.Rows)
                    {
                        // 그리드 행추가
                        pDataGridView.Rows.Add();

                        //pDataGridView[0, pDataGridView.Rows.Count - 1].Value = pDataGridView.Rows.Count - 1;

                        // 컬럼 루프
                        for (int colCount = 0; colCount <= pDataGridView.Columns.Count - 1; colCount++)
                        {
                            if (pDataGridView.Columns[colCount].DataPropertyName != "check_yn")
                            {
                                pDataGridView[colCount, pDataGridView.Rows.Count - 1].Value =
                                    //dataGridViewCampus.Rows[dataGridViewCampus.Rows.Count - 1].Cells[colCount].Value = 
                                    row[pDataGridView.Columns[colCount].DataPropertyName].ToString();                                
                                pDataGridView[colCount, pDataGridView.Rows.Count - 1].Value =                           
                                    row[pDataGridView.Columns[colCount].DataPropertyName].ToString();
                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }


        /// <summary>
        /// pSqlCommand 객체에 쿼리를 정의한다.
        /// </summary>
        /// <param name="pSqlCommand">SqlCommand 객체</param>
        /// <param name="pQueryKind">사용할 쿼리 구분</param>
        /// <param name="pParameter">파라미터</param>
        /// <returns>SqlCommand</returns>
        /// <history>
        /// 박석제, 2014-09-24, 생성
        /// </history>
        private SqlCommand CreateSql(ref SqlCommand pSqlCommand, string pQueryKind, string[] pParameter = null)
        {
            pSqlCommand = new SqlCommand();
            //메인화면
            string businessCD = comboBoxCampusType.SelectedValue.ToString();
            string cpno = comboBoxCampus.SelectedValue.ToString();
            //콩알관리
            string businessCDPoint = comboBoxCampusTypePoint.SelectedValue.ToString();
            string cpnoPoint = comboBoxCampusPoint.SelectedValue.ToString();
            string schoolCDPoint = comboBoxSchoolCDPoint.SelectedValue.ToString();
            string pointCode = comboBoxPointCode.SelectedValue.ToString();
            //차시관리
            string businessCDStudy = comboBoxCampusTypeStudy.SelectedValue.ToString();
            string cpnoStudy = comboBoxCampusStudy.SelectedValue.ToString();
            string yyyyStudy = comboBoxYyyyStudy.SelectedValue.ToString();
            string schoolCDStudy = comboBoxSchoolCDStudy.SelectedValue.ToString();
            string termCDStudy = this._common.IsNull(comboBoxTermCDStudy.SelectedValue);
            //string termCDStudy = comboBoxTermCDStudy.SelectedValue.ToString();
            string useYNStudy = this._common.IsNull(comboBoxUseYNStudy.SelectedValue);
            //미결사용자 등록
            string businessCDByPass = comboBoxCampusTypeByPass.SelectedValue.ToString();
            string cpnoByPass = comboBoxCampusByPass.SelectedValue.ToString();
            
                

            switch (pQueryKind)
            {
                case "select_campus":
                    // 캠퍼스 목록 조회
                    pSqlCommand.CommandText = @"
                     SELECT B.cp_group_nm 
                          , A.cpnm
                		  , CASE A.business_cd WHEN 'DD' THEN '직영'
                                               WHEN 'FA' THEN 'FC'
                                               ELSE 'CP'
                            END business_cd
                		  , A.cpno
                		  , A.cpid
                		  , B.login_char
                		  , B.db_link
                          , B.cp_group_id
                          , A.phone
                		FROM tls_campus AS A
                   LEFT JOIN tls_campus_group AS B
                		  ON A.cp_group_id = B.cp_group_id
                	   WHERE A.use_yn = 'Y' ";
                    if (!string.IsNullOrEmpty(businessCD))
                    {
                        pSqlCommand.CommandText += @"
                         AND A.business_cd = '" + businessCD + "' ";
                    }
                    if (!string.IsNullOrEmpty(cpno))
                    {
                        pSqlCommand.CommandText += @"
                         AND A.cpno = '" + cpno + "' ";
                    }
                    if (!string.IsNullOrEmpty(textBoxCampus.Text))
                    {
                        pSqlCommand.CommandText += @"
                         AND (A.cpnm LIKE '%" + textBoxCampus.Text + "%' ";
                    }
                    if (!string.IsNullOrEmpty(textBoxCampus.Text))
                    {
                        pSqlCommand.CommandText += @"
                         OR B.cp_group_nm LIKE '%" + textBoxCampus.Text + "%') ";
                    }
                    pSqlCommand.CommandText += @"
                       ORDER BY (CASE BUSINESS_CD 
					                  WHEN 'DD' THEN 1
					                  WHEN 'FA' THEN 2
					                  WHEN 'CP' THEN 3
				                 END) 
                               , B.cp_group_nm DESC, A.cpnm ";
                    textBoxCampus.Text = "";
                    textBoxClassNM.Text = "";
                    toolStripTextBoxCampusInfo.Text = "";
                    this.dateTimePickerImportCampusSdate.Value = DateTime.Now;
                    this.dateTimePickerImportCampusEdate.Value = DateTime.Now;
                    break;

                case "select_employee":
                    // 직원 목록 조회
                    pSqlCommand.CommandText = @"
                       SELECT A.member_id
                            , A.usernm
                            , A.login_id
                            , A.login_pwd
                            , UE.enter_date
                            , UE.retire_date
                            , A.use_yn
                            , A.tutor_yn
                            , (SELECT name from tls_web_code WHERE cdmain = 'auth' and cdsub = A.auth_cd) AS AUTH_CD
                            , A.userid
                            , B.cpno
                            , (SELECT REPLACE(cpnm, '캠퍼스', '') FROM tls_campus WHERE cpno = B.cpno) AS CPNM
                         FROM tls_member AS A
                    LEFT JOIN " + GetCellValue(dataGridViewCampus, dataGridViewCampus.CurrentCell.RowIndex, "db_link") + @".DBO.V_u2m_employee AS UE
                           ON A.member_id = UE.emp_id
                    LEFT JOIN tls_cam_member as B
                           ON A.userid = B.userid
                        WHERE B.cpno = " + GetCellValue(dataGridViewCampus, dataGridViewCampus.CurrentCell.RowIndex, "cpno") + @"
                          AND A.auth_cd <> 'S'
                          AND (UE.retire_date = '' OR UE.retire_date IS NULL)                   
                        ORDER BY A.use_yn desc, A.tutor_yn desc, auth_cd, A.usernm
                    ";
                    textBoxClassNM.Text = "";
                    toolStripTextBoxCampusInfo.Text = "";
                    break;

                case "select_employee_all":
                    // 특정 직원 목록 조회
                    pSqlCommand.CommandText = @"
                       SELECT A.member_id
                            , A.usernm
                            , A.login_id
                            , A.login_pwd
                            , B.sdate AS enter_date
                            , B.edate AS retire_date
                            , A.use_yn
                            , A.tutor_yn
                            , (SELECT name from tls_web_code WHERE cdmain = 'auth' and cdsub = A.auth_cd) AS AUTH_CD
                            , A.userid
                            , B.cpno
                            , (SELECT REPLACE(cpnm, '캠퍼스', '') FROM tls_campus WHERE cpno = B.cpno) AS CPNM
                         FROM tls_member AS A                   
                   INNER JOIN tls_cam_member as B
                           ON A.userid = B.userid
                        WHERE A.auth_cd <> 'S'
                          AND (B.edate = '' OR B.edate IS NULL) ";
                    if (!string.IsNullOrEmpty(textBoxUserNm.Text))
                    {
                        pSqlCommand.CommandText += @"
                         AND (A.usernm LIKE '%" + textBoxUserNm.Text + "%' ";
                    }
                    if (!string.IsNullOrEmpty(textBoxUserNm.Text))
                    {
                        pSqlCommand.CommandText += @"
                         OR A.login_id = '" + textBoxUserNm.Text + "' ";
                    }
                    if (!string.IsNullOrEmpty(textBoxUserNm.Text))
                    {
                        pSqlCommand.CommandText += @"
                         OR A.userid LIKE '" + textBoxUserNm.Text + "') ";
                    }
                    pSqlCommand.CommandText += @"
                        ORDER BY cpnm, A.use_yn DESC, A.tutor_yn DESC, A.usernm
                    ";
                    textBoxUserNm.Text = "";
                    textBoxClassNM.Text = "";
                    toolStripTextBoxCampusInfo.Text = "";
                    break;
                case "select_class_employee":
                    // 수업교사 반 목록 조회
                    pSqlCommand.CommandText = @"
	                   SELECT B.class_id
	                        , B.clno
	                        , B.clnm
	                        , B.point
	                        , B.mpoint
	  		                , D.name
	                        , C.usernm                            							  
                            , A.cpno							
							, CASE B.school_cd WHEN 91 THEN '초등'
                                               WHEN 92 THEN '중등'
                                               WHEN 93 THEN '고등'                                               
                              END AS SCHOOL_CD
                            , (SELECT name FROM tls_web_code WHERE cdsub = B.grade_cd AND use_yn = 'Y') AS GRADE_CD                              
							, (SELECT COUNT(clno) FROM tls_class_study WHERE clno = B.clno
							      AND CONVERT(VARCHAR(8), GETDATE(), 112) BETWEEN sdate AND edate) AS STUDY
						    , (SELECT COUNT(clno) FROM tls_class_user WHERE clno = B.clno AND auth_cd = 'S'
							      AND (end_date = '' OR end_date IS NULL OR CONVERT(VARCHAR(8), GETDATE(), 112) BETWEEN start_date AND end_date)) AS USER_CNT
	                     FROM tls_class_user AS A 
                    LEFT JOIN tls_class AS B 
	                       ON A.clno = B.clno
                    LEFT JOIN tls_member AS C
	                       ON B.class_tid = C.userid				    
					LEFT JOIN tls_web_code AS D
					       ON B.grade_cd = D.cdsub
	                    WHERE A.cpno = " + GetCellValue(dataGridViewEmployee, dataGridViewEmployee.CurrentCell.RowIndex, "cpno") + @" 
                         AND A.userid = " + GetCellValue(dataGridViewEmployee, dataGridViewEmployee.CurrentCell.RowIndex, "userid") + @"
                         AND (B.edate = '' OR B.edate IS NULL OR B.edate >= CONVERT(VARCHAR(8), GETDATE(), 112))
                         AND (A.end_date = '' OR A.end_date IS NULL OR CONVERT(VARCHAR(8), GETDATE(), 112) BETWEEN A.start_date and A.end_date)
                         AND B.use_yn = 'Y'											   
	                   ORDER BY B.school_cd, B.clnm
                    ";
                    toolStripTextBoxCampusInfo.Text = "";
                    break;

                case "select_class_employee_all":
                    //특정 반 목록 조회
                    pSqlCommand.CommandText = @"
                       SELECT A.class_id
	                        , A.clno
	                        , A.clnm
	                        , A.point
	                        , A.mpoint
                            , B.usernm
                            , A.cpno
	  		                , CASE A.school_cd WHEN 91 THEN '초등'
                                               WHEN 92 THEN '중등'
                                               WHEN 93 THEN '고등'                                               
                              END AS SCHOOL_CD
                            , (SELECT name FROM tls_web_code WHERE cdsub = A.grade_cd AND use_yn = 'Y') AS GRADE_CD                              	                        
                            , (SELECT COUNT(clno) FROM tls_class_study WHERE clno = A.clno
							      AND CONVERT(VARCHAR(8), GETDATE(), 112) BETWEEN sdate AND edate) AS STUDY
						    , (SELECT COUNT(clno) FROM tls_class_user WHERE clno = A.clno AND auth_cd = 'S'
							      AND (end_date = '' OR end_date IS NULL OR CONVERT(VARCHAR(8), GETDATE(), 112) BETWEEN start_date AND end_date)) AS USER_CNT
	                     FROM tls_class AS A
					LEFT JOIN tls_member AS B
					       ON A.CLASS_TID = B.userid                    
						WHERE A.cpno = " + GetCellValue(dataGridViewEmployee, dataGridViewEmployee.CurrentCell.RowIndex, "cpno") + @"
						  AND (A.edate = '' OR A.edate IS NULL OR A.edate >= CONVERT(VARCHAR(8), GETDATE(), 112))
                    ";
                    if (!string.IsNullOrEmpty(textBoxClassNM.Text))
                    {
                        pSqlCommand.CommandText += @"
                         AND A.clnm LIKE '%" + textBoxClassNM.Text + "%' ";
                    }
                    pSqlCommand.CommandText += @"						 
	                    ORDER BY A.school_cd, A.clnm
                    ";
                    toolStripTextBoxCampusInfo.Text = "";
                    break;

                case "select_class_student":
                    //반 학생 목록 조회
                    pSqlCommand.CommandText = @"
                       SELECT A.userid
	                        , C.usernm
                            , A.cpno  
                            , B.clnm
                            , C.login_id
                            , C.login_pwd
                            , A.start_date
                            , A.end_date
                            , (SELECT COUNT(userid) FROM tls_member_study WHERE userid = C.userid
							      AND CONVERT(VARCHAR(8), GETDATE(), 112) BETWEEN sdate AND edate) AS STUDY
                            , (SELECT cpnm from tls_campus WHERE cpno = A.cpno) AS CPNM                        
	                     FROM tls_class_user AS A 
                    LEFT JOIN tls_class AS B 
                           ON A.clno = B.clno
                    LEFT JOIN tls_member AS C
                           ON A.userid = C.userid
                        WHERE B.clno = " + GetCellValue(dataGridViewClassEmployee, dataGridViewClassEmployee.CurrentCell.RowIndex, "clno") + @"
                          AND A.auth_cd = 'S'
                          AND (A.end_date = '' OR A.end_date IS NULL OR CONVERT(VARCHAR(8), GETDATE(), 112) BETWEEN A.start_date and A.end_date)
                        ORDER BY C.usernm
                    ";
                    break;
                case "select_class_student_all":
                    //캠퍼스 특정 학생 검색
                    pSqlCommand.CommandText = @"
                       SELECT A.userid
	                        , C.usernm
                            , A.cpno  
                            , B.clnm
                            , C.login_id
                            , C.login_pwd
                            , A.start_date
                            , A.end_date
                            , (SELECT COUNT(userid) FROM tls_member_study WHERE userid = C.userid
							      AND CONVERT(VARCHAR(8), GETDATE(), 112) BETWEEN sdate AND edate) AS STUDY
                            , (SELECT cpnm from tls_campus WHERE cpno = A.cpno) AS CPNM                                                 
	                     FROM tls_class_user AS A 
                    LEFT JOIN tls_class AS B 
                           ON A.clno = B.clno
                    LEFT JOIN tls_member AS C
                           ON A.userid = C.userid
                        WHERE A.auth_cd = 'S'
                          AND A.cpno = " + GetCellValue(dataGridViewEmployee, dataGridViewEmployee.CurrentCell.RowIndex, "cpno") + @"
                          AND (A.end_date = '' OR A.end_date IS NULL OR A.end_date >= CONVERT(VARCHAR(8), GETDATE(), 112) ) ";                        
                    if (!string.IsNullOrEmpty(textBoxStudentNM.Text))
                    {
                        pSqlCommand.CommandText += @"
                         AND C.usernm LIKE '%" + textBoxStudentNM.Text + "%' ";
                    }
                    pSqlCommand.CommandText += @"						 
	                    ORDER BY B.cpno, C.usernm
                    ";
                    textBoxStudentNM.Text = "";
                    break;

                case "select_class_edu_student_all":
                    //드림+ 학생 검색
                    pSqlCommand.CommandText = @"
                       SELECT (SELECT cpnm FROM tls_campus WHERE cpno = " + GetCellValue(dataGridViewCampus, dataGridViewCampus.CurrentCell.RowIndex, "cpno") + @") AS CPNM                                                 
                            , A.userid
                            , US.name                              
                            , USC.class_name
                            , US.login_id
                            , US.login_pwd 
                            , US.grade_name                           
                            , US.start_date
                            , US.end_date
                            , USC.start_date AS C_START_DATE
                            , USC.end_date AS C_END_DATE
                            , US.student_id
                            , USC.class_id
                            , US.std_modify_date                            
	                     FROM " + GetCellValue(dataGridViewCampus, dataGridViewCampus.CurrentCell.RowIndex, "db_link") + @".DBO.V_u2m_student AS US WITH(nolock)
                    LEFT JOIN " + GetCellValue(dataGridViewCampus, dataGridViewCampus.CurrentCell.RowIndex, "db_link") + @".DBO.V_u2m_student_class AS USC WITH(nolock)
                           ON US.acad_id = USC.acad_id AND US.student_id = USC.student_id AND US.login_id = USC.login_id
                    LEFT JOIN tls_member AS A
                           ON US.student_id = A.member_id AND US.login_id = A.login_id 
                        WHERE US.acad_id = " + GetCellValue(dataGridViewCampus, dataGridViewCampus.CurrentCell.RowIndex, "cpid") + @"
                    ";                        
                    if (!string.IsNullOrEmpty(textBoxEduStudentNM.Text))
                    {
                        pSqlCommand.CommandText += @"
                         AND (US.name LIKE '%" + textBoxEduStudentNM.Text + "%' ";
                    }
                    if (!string.IsNullOrEmpty(textBoxEduStudentNM.Text))
                    {
                        pSqlCommand.CommandText += @"
                         OR US.LOGIN_ID = '" + textBoxEduStudentNM.Text + "') ";
                    }
                    pSqlCommand.CommandText += @"						 
	                    ORDER BY A.userid, US.name, USC.start_date DESC
                    ";
                    textBoxEduStudentNM.Text = "";
                    break;

                case "select_campus_point":
                    // 콩알관리 탭 캠퍼스(콩알) 목록 조회
                    pSqlCommand.CommandText = @"
                     SELECT A.cpnm                           
                		  , CASE A.business_cd WHEN 'DD' THEN '직영'
                                               WHEN 'FA' THEN 'FC'
                                               ELSE 'CP'
                            END business_cd
                		  , SUM(C.mpoint) AS POINT 
                          , A.cpno                        
                		FROM tls_campus AS A
                   LEFT JOIN tls_campus_group AS B
                		  ON A.cp_group_id = B.cp_group_id
                   LEFT JOIN tls_class AS C
                          ON A.cpno = C.cpno
                	   WHERE A.use_yn = 'Y' 
                         AND C.use_yn = 'Y'
                         AND (C.edate = '' or C.edate = '' or C.edate >= CONVERT(VARCHAR(8), GETDATE(), 112))
                             ";
                    if (!string.IsNullOrEmpty(businessCDPoint))
                    {
                        pSqlCommand.CommandText += @"
                         AND A.business_cd = '" + businessCDPoint + "' ";
                    }
                    if (!string.IsNullOrEmpty(cpnoPoint))
                    {
                        pSqlCommand.CommandText += @"
                         AND A.cpno = '" + cpnoPoint + "' ";
                    }                    
                    pSqlCommand.CommandText += @"
                       GROUP BY A.cpnm, A.cpno, A.business_cd
                       ORDER BY A.business_cd DESC,  A.cpnm ";
                    break;

                case "select_class_point":
                    //콩알관리 탭 반 콩알 목록 조회
                    pSqlCommand.CommandText = @"
                       SELECT A.clnm
                            , A.point
                            , A.mpoint
                            , (SELECT COUNT(userid)FROM tls_class_user WHERE clno = A.clno AND cpno = A.cpno AND auth_cd = 'S'
                                  AND (end_date = '' OR end_date IS NULL OR CONVERT(CHAR,GETDATE(),112) BETWEEN start_date AND end_date)) AS CL_USER
                            , A.udatetime
                            , A.school_cd
                            , A.cpno
                            , A.clno
	                     FROM tls_class AS A
                        WHERE A.cpno = " + GetCellValue(dataGridViewCampusPoint, dataGridViewCampusPoint.CurrentCell.RowIndex, "cpno") + @"
                          AND A.use_yn = 'Y'
                          AND (A.edate = '' or A.edate = '' or A.edate >= CONVERT(VARCHAR(8), GETDATE(), 112)) ";                    
                    if (!string.IsNullOrEmpty(schoolCDPoint))
                    {
                        pSqlCommand.CommandText += @"
                         AND school_cd = '" + schoolCDPoint + "' ";
                    }                    
                    pSqlCommand.CommandText += @"
                       ORDER BY clnm desc ";
                    break;

                case "select_new_class_point":
                    //콩알관리 탭 신규반 콩알 목록 조회
                    pSqlCommand.CommandText = @"
                       SELECT A.clnm
                            , A.point
                            , A.mpoint
                            , (SELECT COUNT(userid)FROM tls_class_user WHERE clno = A.clno AND cpno = A.cpno AND auth_cd = 'S'
                                  AND (end_date = '' OR end_date IS NULL OR CONVERT(CHAR,GETDATE(),112) BETWEEN start_date AND end_date)) AS CL_USER
                            , A.udatetime
                            , A.school_cd
                            , A.cpno
                            , A.clno
	                     FROM tls_class AS A
                        WHERE A.cpno = " + GetCellValue(dataGridViewCampusPoint, dataGridViewCampusPoint.CurrentCell.RowIndex, "cpno") + @"
                          AND A.use_yn = 'Y'
                          AND (A.edate = '' or A.edate = '' or A.edate >= CONVERT(VARCHAR(8), GETDATE(), 112))
                          AND (point = 0 OR point is null OR point = '' )
                          AND (mpoint = 0 OR mpoint is null OR mpoint = '')
                          AND (SELECT COUNT(userid)FROM tls_class_user WHERE clno = A.clno AND cpno = A.cpno AND auth_cd = 'S'
                                  AND (end_date = '' OR end_date IS NULL OR CONVERT(CHAR,GETDATE(),112) BETWEEN start_date AND end_date)) > 0 ";                    
                    if (!string.IsNullOrEmpty(schoolCDPoint))
                    {
                        pSqlCommand.CommandText += @"
                         AND school_cd = '" + schoolCDPoint + "' ";
                    }                    
                    pSqlCommand.CommandText += @"
                       ORDER BY clnm desc ";
                    break;

                case "select_student_point":
                    //해당 반의 학생 콩알 목록 조회
                    pSqlCommand.CommandText = @"
                       SELECT D.cpnm
                            , B.clnm
	                        , C.usernm
                            , C.point
                            , (SELECT SUM(point) FROM tls_point_user WHERE userid = C.userid
							                      AND pcode <> 23  ) AS ALL_POINT
                            , A.cpno                            
                            , A.userid
                            , C.login_id
                            , C.login_pwd
	                     FROM tls_class_user AS A 
                    LEFT JOIN tls_class AS B 
                           ON A.clno = B.clno
                    LEFT JOIN tls_member AS C
                           ON A.userid = C.userid
                    LEFT JOIN tls_campus AS D
                           ON A.cpno = D.cpno
                        WHERE A.cpno = '" + GetCellValue(dataGridViewCampusPoint, dataGridViewCampusPoint.CurrentCell.RowIndex, "cpno") + @"'
                          AND A.clno = '" + GetCellValue(dataGridViewClassPoint, dataGridViewClassPoint.CurrentCell.RowIndex, "clno") + @"'
                          AND A.auth_cd = 'S'
                          AND (A.end_date = '' OR A.end_date IS NULL OR CONVERT(VARCHAR(8), GETDATE(), 112) BETWEEN A.start_date AND A.end_date ) ";

                     if (!string.IsNullOrEmpty(businessCDPoint))
                    {
                        pSqlCommand.CommandText += @"
                         AND D.business_cd = '" + businessCDPoint + "' ";
                    }
                    if (!string.IsNullOrEmpty(cpnoPoint))
                    {
                        pSqlCommand.CommandText += @"
                         AND A.cpno = '" + cpnoPoint + "' ";
                    }
                    if (!string.IsNullOrEmpty(schoolCDPoint))
                    {
                        pSqlCommand.CommandText += @"
                         AND B.school_cd = '" + schoolCDPoint + "' ";
                    }       
                    pSqlCommand.CommandText += @"                        
                        ORDER BY D.cpnm, B.clnm, C.usernm ";
                   
                    
                    break;
                case "select_student_point_all":
                    //학생 콩알 목록 조회
                    pSqlCommand.CommandText = @"
                       SELECT D.cpnm
                            , B.clnm
	                        , C.usernm
                            , C.point
                            , (SELECT SUM(point) FROM tls_point_user WHERE userid = C.userid
							                      AND pcode <> 23  ) AS ALL_POINT
                            , A.cpno                            
                            , A.userid
                            , C.login_id
                            , C.login_pwd
	                     FROM tls_class_user AS A 
                    LEFT JOIN tls_class AS B 
                           ON A.clno = B.clno
                    LEFT JOIN tls_member AS C
                           ON A.userid = C.userid
                    LEFT JOIN tls_campus AS D
                           ON A.cpno = D.cpno
                        WHERE A.auth_cd = 'S'
                          AND (A.end_date = '' OR A.end_date IS NULL OR A.end_date >= CONVERT(VARCHAR(8), GETDATE(), 112) ) ";

                    if (!string.IsNullOrEmpty(businessCDPoint))
                    {
                        pSqlCommand.CommandText += @"
                         AND D.business_cd = '" + businessCDPoint + "' ";
                    }
                    if (!string.IsNullOrEmpty(cpnoPoint))
                    {
                        pSqlCommand.CommandText += @"
                         AND A.cpno = '" + cpnoPoint + "' ";
                    }
                    if (!string.IsNullOrEmpty(schoolCDPoint))
                    {
                        pSqlCommand.CommandText += @"
                         AND B.school_cd = '" + schoolCDPoint + "' ";
                    }                    
                    
                    if (!string.IsNullOrEmpty(textBoxStudentNMPoint.Text))
                    {
                        pSqlCommand.CommandText += @"
                        AND C.usernm like '%" + textBoxStudentNMPoint.Text + "%' ";
                    }


                    pSqlCommand.CommandText += @"                        
                        ORDER BY D.cpnm, B.clnm, C.usernm ";
                    textBoxStudentNMPoint.Text = "";

                    break;
                case "select_student_point_save":
                    //학생 콩알 내역 목록 조회
                    pSqlCommand.CommandText = @"
                       		SELECT B.name			 
			                     , A.userid
			                     , A.clno
			                     , A.point
			                     , (SELECT usernm FROM tls_member WHERE userid = A.rid) AS RID
			                     , A.rdatetime
			                     , A.cpno
                                 , A.pcode
                                 , A.pno
	                          FROM tls_point_user AS A
                         LEFT JOIN tls_point_code AS B
	     	                    ON A.pcode = B.PCODE
		                     WHERE A.userid = '" + GetCellValue(dataGridViewStudentPoint, dataGridViewStudentPoint.CurrentCell.RowIndex, "userid") + @"'
	                         ORDER BY A.rdatetime DESC ";

                    textBoxStudentPoint.Text = "";
                    break;

                case "insert_student_point":
                    //학생 콩알 내역 적립
                    pSqlCommand.CommandText = @"
                       		 INSERT INTO TLS_POINT_USER
                                       ( USERID
                                       , PCODE                                
                                       , POINT           
                                       , DEL_YN
                                       , RID
                                       , RDATETIME)
                                 VALUES (";
                                        if (dataGridViewStudentPoint.Rows.Count > 0 && dataGridViewStudentPoint.CurrentCell != null)
                                        {
                                            pSqlCommand.CommandText += @"
                                            '" + GetCellValue(dataGridViewStudentPoint, dataGridViewStudentPoint.CurrentCell.RowIndex, "userid") + "' ";
                                        }
                                        if (!string.IsNullOrEmpty(pointCode))
                                        {
                                            pSqlCommand.CommandText += @"
                                               , " + pointCode + " ";
                                        }
                                        pSqlCommand.CommandText += @" ";
                                        if (!string.IsNullOrEmpty(textBoxStudentPoint.Text))
                                        {
                                            pSqlCommand.CommandText += @"
                                               , CONVERT(INT, " + textBoxStudentPoint.Text + ") ";
                                        }
                                        pSqlCommand.CommandText += @"
                                       ,'N'
                                       , 1                                                           
                                       ,getdate()) ";
                                        textBoxStudentPoint.Text = "";
                    break;

                case "select_point_manager":
                    //콩알 관리자 조회
                    pSqlCommand.CommandText = @"
                       		 SELECT REPLACE(B.cpnm, '캠퍼스', '') AS CPNM
			                      , C.usernm
                                  , A.point
			                      , A.mpoint
			                      , A.userid
			                      , A.cpno
			                      , A.auth_cd			                      
                                  , A.use_yn
			                      , (SELECT usernm FROM tls_member WHERE userid = A.rid) AS RID
			                   FROM tls_point_manager AS A
	                      LEFT JOIN tls_campus AS B
		                         ON A.cpno = B.cpno 
	                      LEFT JOIN tls_member AS C
			                     ON A.userid = C.userid
                                ORDER BY A.point DESC ";
                    break;

                case "insert_point_manager":
                    //콩알 관리자 등록
                    pSqlCommand.CommandText = @"
                       		 INSERT INTO TLS_POINT_MANAGER
                                       ( USERID
                                       , CPNO
                                       , AUTH_CD
                                       , POINT
                                       , MPOINT
                                       , USE_YN
                                       , RID
                                       , RDATETIME
                                       , UID
                                       , UDATETIME)
                                 VALUES
                                       (";
                                        if (!string.IsNullOrEmpty(textBoxPointManagerUserid.Text))
                                        {
                                           pSqlCommand.CommandText += @"
                                            " + textBoxPointManagerUserid.Text + " ";
                                        }
                                       pSqlCommand.CommandText += @"
                                       ,";
                                       if (!string.IsNullOrEmpty(textBoxPointManagerCpno.Text))
                                       {
                                          pSqlCommand.CommandText += @"
                                            " + textBoxPointManagerCpno.Text + " ";
                                       }
                                       pSqlCommand.CommandText += @"
                                       ,'D'
                                       , 5000
                                       , 5000
                                       ,'Y'
                                       ,1
                                       ,getdate()
                                       ,1
                                       ,getdate())                     
                   
                       		 SELECT REPLACE(B.cpnm, '캠퍼스', '') AS CPNM
			                      , C.usernm
                                  , A.point
			                      , A.mpoint
			                      , A.userid
			                      , A.cpno
			                      , A.auth_cd			                      
                                  , A.use_yn
			                      , (SELECT usernm FROM tls_member WHERE userid = A.rid) AS RID
			                   FROM tls_point_manager AS A
	                      LEFT JOIN tls_campus AS B
		                         ON A.cpno = B.cpno 
	                      LEFT JOIN tls_member AS C
			                     ON A.userid = C.userid ";
                          if (!string.IsNullOrEmpty(textBoxPointManagerCpno.Text))
                          {
                             pSqlCommand.CommandText += @"
                              WHERE A.cpno = " + textBoxPointManagerCpno.Text + " ";
                          }                                                                
                          if (!string.IsNullOrEmpty(textBoxPointManagerUserid.Text))
                          {
                             pSqlCommand.CommandText += @"
                               AND A.userid = " + textBoxPointManagerUserid.Text + " ";
                          }
                          pSqlCommand.CommandText += @"                             
                             ORDER BY A.udatetime DESC ";
                          textBoxPointManagerCpno.Text = "";
                          textBoxPointManagerUserid.Text = "";
                    break;
                case "select_class":
                    //차시관리 반 목록조회
                    pSqlCommand.CommandText = @"                       		 
                       SELECT A.clnm
	                        , (SELECT COUNT(clno) FROM tls_class_study 
                                WHERE cpno = A.cpno AND clno = A.clno
                                  AND (CONVERT(CHAR,GETDATE(),112) BETWEEN sdate AND edate)) AS STUDY
							, (SELECT COUNT(userid) FROM tls_class_user 
								WHERE cpno = A.cpno AND clno = a.clno AND auth_cd = 's'
								  AND (end_date = '' OR end_date IS NULL OR CONVERT(CHAR,GETDATE(),112) BETWEEN start_date AND end_date)) AS USER_CNT
                            , C.cpnm
                            , A.cpno  
							, A.clno
                            , A.school_cd                           						
                         FROM tls_class AS A                                                                    
				    LEFT JOIN tls_term AS B
					       ON A.cpno = B.cpno
                    LEFT JOIN tls_campus AS C
                           ON A.cpno = C.cpno
						   WHERE A.use_yn = 'Y'
                    ";
                    if (!string.IsNullOrEmpty(businessCDStudy))
                    {
                        pSqlCommand.CommandText += @"
                         AND C.business_cd = '" + businessCDStudy + "' ";
                    }
                    if (!string.IsNullOrEmpty(cpnoStudy))
                    {
                        pSqlCommand.CommandText += @"
                         AND C.cpno = '" + cpnoStudy + "' ";
                    }
                    if (!string.IsNullOrEmpty(yyyyStudy))
                    {
                        pSqlCommand.CommandText += @"
                         AND B.yyyy = '" + yyyyStudy + "' ";
                    }
                    if (!string.IsNullOrEmpty(schoolCDStudy))
                    {
                        pSqlCommand.CommandText += @"
                         AND A.school_cd = '" + schoolCDStudy + "' ";
                    }                    
                    if (!string.IsNullOrEmpty(termCDStudy))
                    {
                        pSqlCommand.CommandText += @"
                         AND B.term_cd = '" + termCDStudy + "' ";
                    }
                    if (!string.IsNullOrEmpty(textBoxClassStudy.Text))
                    {
                        pSqlCommand.CommandText += @"
                         AND A.clnm like '%" + textBoxClassStudy.Text + "%' ";
                    }
                    if (!string.IsNullOrEmpty(textBoxCampusStudy.Text))
                    {
                        pSqlCommand.CommandText += @"
                         AND C.cpnm like '%" + textBoxCampusStudy.Text + "%' ";
                    }
                    pSqlCommand.CommandText += @"
                    GROUP BY A.cpno, C.cpnm, A.school_cd, A.clnm,  A.clno
					ORDER BY A.cpno, A.school_cd, A.clnm
                         ";
                    textBoxClassStudy.Text = "";
                    break;

                case "select_student":
                    //차시관리 반 학생 조회
                    pSqlCommand.CommandText = @"
                       SELECT A.userid
	                        , C.usernm
                            , (SELECT COUNT(userid) FROM tls_member_study 
                                WHERE cpno = A.cpno AND userid = A.userid
                                  AND (CONVERT(CHAR,GETDATE(),112) BETWEEN sdate AND edate)) AS STUDY							
                            , A.cpno  
                            , A.clno
                            , C.login_id
                            , C.login_pwd 
                            , D.cpnm                         
	                     FROM tls_class_user AS A 
                    LEFT JOIN tls_class AS B 
                           ON A.clno = B.clno
                    LEFT JOIN tls_member AS C
                           ON A.userid = C.userid
                    LEFT JOIN tls_campus AS D
                           ON B.cpno = D.cpno
                        WHERE B.clno = " + GetCellValue(dataGridViewClass, dataGridViewClass.CurrentCell.RowIndex, "clno") + @"
                          AND A.auth_cd = 'S'
                          AND (A.end_date = '' OR A.end_date IS NULL OR A.end_date >= CONVERT(VARCHAR(8), GETDATE(), 112) )
                        ORDER BY C.usernm
                    ";                    
                    break;
                case "select_student_all":
                    //차시관리 반 학생 조회
                    pSqlCommand.CommandText = @"
                       SELECT A.userid
	                        , C.usernm
                            , (SELECT COUNT(userid) FROM tls_member_study 
                                WHERE cpno = A.cpno AND userid = A.userid
                                  AND (CONVERT(CHAR,GETDATE(),112) BETWEEN sdate AND edate)) AS STUDY
                            , A.cpno  
                            , A.clno
                            , C.login_id
                            , C.login_pwd
                            , D.cpnm                          
	                     FROM tls_class_user AS A 
                    LEFT JOIN tls_class AS B 
                           ON A.clno = B.clno
                    LEFT JOIN tls_member AS C
                           ON A.userid = C.userid
                    LEFT JOIN tls_campus AS D
                           ON B.cpno = D.cpno
                        WHERE A.auth_cd = 'S'
                          AND (A.end_date = '' OR A.end_date IS NULL OR A.end_date >= CONVERT(VARCHAR(8), GETDATE(), 112) )
                        ";
                    if (!string.IsNullOrEmpty(businessCDStudy))
                    {
                        pSqlCommand.CommandText += @"
                         AND D.business_cd = '" + businessCDStudy + "' ";
                    }
                    if (!string.IsNullOrEmpty(cpnoStudy))
                    {
                        pSqlCommand.CommandText += @"
                         AND D.cpno = '" + cpnoStudy + "' ";
                    }
                    if (!string.IsNullOrEmpty(textBoxStudentStudy.Text))
                    {
                        pSqlCommand.CommandText += @"
                         AND C.usernm like '%" + textBoxStudentStudy.Text + "%' ";
                    }
                    pSqlCommand.CommandText += @"
                        ORDER BY C.usernm
                    ";
                    textBoxClassStudy.Text = "";
                    textBoxStudentStudy.Text = "";
                    break;

                 case "select_bypass":
                    //미결사용자 학생 조회
                    pSqlCommand.CommandText = @"   
                        SELECT A.request_seq	
	                         , A.cpno
	                         , A.cpid
	                         , A.cpnm
  	                         , A.empno
	                         , A.empnm
	                         , A.userid
	                         , A.usernm
	                         , STUFF(STUFF(A.use_from, 5, 0, '-'), 8, 0, '-') AS USE_FROM
	                         , STUFF(STUFF(A.use_to, 5, 0, '-'), 8, 0, '-') AS USE_TO
	                         , A.counsel_date
	                         , A.counsel_nm
	                         , A.reason
                             , CASE B.business_cd WHEN 'DD' THEN '직영'
                                                  WHEN 'FA' THEN 'FC'
                                                  ELSE 'CP'
                                END business_cd
                             , (SELECT login_id FROM tls_member WHERE userid = A.userid) AS LOGIN_ID
                             , (SELECT login_pwd FROM tls_member WHERE userid = A.userid) AS LOGIN_PWD   
                          FROM tls_bypass AS A
                     LEFT JOIN tls_campus AS B
                            ON A.cpno = B.cpno
                         WHERE 1=1 ";
                    if (!string.IsNullOrEmpty(businessCDByPass))
                    {
                        pSqlCommand.CommandText += @"
                         AND B.business_cd = '" + businessCDByPass + "' ";
                    }
                    if (!string.IsNullOrEmpty(cpnoByPass))
                    {
                        pSqlCommand.CommandText += @"
                         AND A.cpno = '" + cpnoByPass + "' ";
                    }
                    if (!string.IsNullOrEmpty(textBoxStudentNmByPass.Text))
                    {
                        pSqlCommand.CommandText += @"
                         AND A.usernm LIKE '%" + textBoxStudentNmByPass.Text + "%' ";
                    }                   
                    pSqlCommand.CommandText += @"
                       ORDER BY (CASE B.BUSINESS_CD 
					                  WHEN 'DD' THEN 1
					                  WHEN 'FA' THEN 2
					                  WHEN 'CP' THEN 3
				                 END) 
                               , A.cpnm, A.usernm ";

                    textBoxByPassEmpNM2.Text = "";
                    textBoxByPassEmpID.Text = "";
                    textBoxByPassUserNM2.Text = "";
                    textBoxByPassUserID.Text = "";
                    this.dateTimePickerByPassUseFrom.Value = DateTime.Now;
                    this.dateTimePickerByPassUseTo.Value = DateTime.Now;
                    textBoxByPassReason.Text = "";

                    break;

                 case "select_bypass_emp":
                    //미결사용자 신청자명(교사) 조회
                    pSqlCommand.CommandText = @"   
                        SELECT C.cpnm
                             , A.usernm
                             , A.login_id
                             , A.userid
                          FROM tls_member AS A
                     LEFT JOIN tls_cam_member as B
                            ON A.userid = B.userid
                     LEFT JOIN tls_campus AS C
                            ON B.cpno = C.cpno
                         WHERE A.use_yn = 'Y'
                           AND A.auth_cd <> 'S' ";
                    if (!string.IsNullOrEmpty(businessCDByPass))
                    {
                        pSqlCommand.CommandText += @"
                         AND C.business_cd = '" + businessCDByPass + "' ";
                    }
                    if (!string.IsNullOrEmpty(cpnoByPass))
                    {
                        pSqlCommand.CommandText += @"
                         AND B.cpno = '" + cpnoByPass + "' ";
                    }
                    if (!string.IsNullOrEmpty(textBoxByPassEmpNM.Text))
                    {
                        pSqlCommand.CommandText += @"
                         AND A.usernm LIKE '%" + textBoxByPassEmpNM.Text + "%' ";
                    }                   
                    pSqlCommand.CommandText += @"
                       ORDER BY C.cpnm, A.usernm "; 
                    break;

                 case "select_bypass_user":
                    //미결사용자 대상자명(학생) 조회
                    pSqlCommand.CommandText = @"   
                        SELECT C.cpnm 
                             , A.usernm
                             , A.login_id
                             , A.userid
                             , C.cpno
                             , C.cpid
                             , A.login_pwd
                          FROM tls_member AS A
                     LEFT JOIN tls_cam_member as B
                            ON A.userid = B.userid
                     LEFT JOIN tls_campus AS C
                            ON B.cpno = C.cpno
                         WHERE A.use_yn = 'Y'
                           AND  A.auth_cd = 'S' ";
                    if (!string.IsNullOrEmpty(businessCDByPass))
                    {
                        pSqlCommand.CommandText += @"
                         AND C.business_cd = '" + businessCDByPass + "' ";
                    }
                    if (!string.IsNullOrEmpty(cpnoByPass))
                    {
                        pSqlCommand.CommandText += @"
                         AND B.cpno = '" + cpnoByPass + "' ";
                    }
                    if (!string.IsNullOrEmpty(textBoxByPassUserNM.Text))
                    {
                        pSqlCommand.CommandText += @"
                         AND A.usernm LIKE '%" + textBoxByPassUserNM.Text + "%' ";
                    }
                    pSqlCommand.CommandText += @"
                       ORDER BY C.cpnm, A.usernm ";
                    break;

                default:
                    break;

            }

            return pSqlCommand;
        }

        /// <summary>
        /// 그리드에서 특정 행렬의 값을 리턴한다.
        /// </summary>
        /// <param name="pDataGridView">그리드</param>
        /// <param name="pRowIndex">행번호</param>
        /// <param name="pDataPropertyName">컬럼에 바인딩된 데이터베이스 열</param>
        /// <returns>그리드에서 특정 행렬의 값</returns>
        /// <history>
        /// 박석제, 2014-09-24, 생성
        /// </history>
        private string GetCellValue(DataGridView pDataGridView, int pRowIndex, string pDataPropertyName = "")
        {
            string CellValue = "";

            foreach (DataGridViewColumn item in pDataGridView.Columns)
            {
                if (item.DataPropertyName.ToLower() == pDataPropertyName)
                {
                    if (pDataGridView[item.Index, pRowIndex].Value != null)
                    {
                        CellValue = pDataGridView[item.Index, pRowIndex].Value.ToString();  
                    }
                    break;
                }
            }

            return CellValue;
        }
                
        /// <summary>
        /// 반콩알을 적립한다
        /// </summary>
        /// <history>
        /// 박석제, 2014-10-07, 생성
        /// </history>
        private void UpdateClassPoint()
        {
            Boolean isFound = false; // 처리할 자료가 있는지 체크할 변수
            DialogResult result = this._common.MessageBox(MessageBoxIcon.Question, "정말 수정하시겠습니까?");
            if (result == DialogResult.No)
            {
                return;
            }

            SqlCommand sqlCommand = new SqlCommand();
            SqlResult sqlResult = new SqlResult();

            this.Cursor = Cursors.WaitCursor;

            try
            {
                // 컬럼 루프
                for (int rowCount = 0; rowCount <= dataGridViewClassPoint.Rows.Count - 1; rowCount++)
                {
                    if (GetCellValue(dataGridViewClassPoint, rowCount, "check_yn") == "1")
                    {
                        isFound = true;
                        sqlCommand.CommandText += @"
                            UPDATE B SET point = '" + textBoxPoint.Text + @"' * USER_CNT * 20
                                       , mpoint = '" + textBoxPoint.Text + @"' * USER_CNT * 20
                                       , UID = 1
				                       , UDATETIME = GETDATE()
		                      FROM (
			                         SELECT cpno, clno, COUNT(clno) AS USER_CNT
				                       FROM tls_class_user  
			                          WHERE cpno = '" + this._common.GetCellValue(dataGridViewClassPoint, rowCount, "cpno") + @"'
			                            AND clno = '" + this._common.GetCellValue(dataGridViewClassPoint, rowCount, "clno") + @"'
				                        AND auth_cd = 'S' 
				                        AND (end_date = '' OR end_date IS NULL OR CONVERT(VARCHAR(8), GETDATE(), 112) BETWEEN start_date AND end_date)
			                          GROUP BY cpno, clno
			                         ) AS A
		                      LEFT JOIN tls_class AS B
		                        ON A.cpno = B.cpno AND A.clno = B.clno  ";
                    }
                }

                Console.WriteLine(sqlCommand.CommandText);

                if (isFound == true)
                {
                    // 처리할 자료가 있을 경우 쿼리실행
                    this._common.ExecuteNonQuery(sqlCommand, ref sqlResult);

                    if (sqlResult.Success == true)
                    {
                        // 작업 성공시
                        if (sqlResult.AffectedRecords > 0)
                            this._common.MessageBox(MessageBoxIcon.Information, "자료를 수정하였습니다." + Environment.NewLine +
                                string.Format("(수정된 자료건 수 총 : {0}건)", sqlResult.AffectedRecords));
                        else
                            this._common.MessageBox(MessageBoxIcon.Information, "수정된 자료가 없습니다.");
                    }
                    else
                        // 작업 실패시
                        MessageBox.Show(sqlResult.ErrorMsg);
                }
                else
                    // 처리할 자료가 없을 경우
                    this._common.MessageBox(MessageBoxIcon.Information, "수정할 자료가 없습니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlCommand.Dispose();
                this.Cursor = Cursors.Default;
            }
        }
        
        /// <summary>
        /// 학생 콩알내역을 삭제한다.
        /// </summary>
        /// <history>
        /// 박석제, 2014-10-07, 생성
        /// </history>
        private void DeleteStudentPoint()
        {
            Boolean isFound = false; // 처리할 자료가 있는지 체크할 변수
            DialogResult result = this._common.MessageBox(MessageBoxIcon.Question, "정말 삭제하시겠습니까?");
            if (result == DialogResult.No)
            {
                return;
            }

            SqlCommand sqlCommand = new SqlCommand();
            SqlResult sqlResult = new SqlResult();

            this.Cursor = Cursors.WaitCursor;

            try
            {
                // 컬럼 루프
                for (int rowCount = 0; rowCount <= dataGridViewStudentPointSave.Rows.Count - 1; rowCount++)
                {
                    if (GetCellValue(dataGridViewStudentPointSave, rowCount, "check_yn") == "1")
                    {
                        isFound = true;
                        sqlCommand.CommandText += @"
                            DELETE
                              FROM tls_point_user 
                             WHERE userid = '" + this._common.GetCellValue(dataGridViewStudentPointSave, rowCount, "userid") + @"'
                               AND pcode = '" + this._common.GetCellValue(dataGridViewStudentPointSave, rowCount, "pcode") + @"'
                               AND pno = '" + this._common.GetCellValue(dataGridViewStudentPointSave, rowCount, "pno") + @"' ";                                                    
                    }
                }

                Console.WriteLine(sqlCommand.CommandText);

                if (isFound == true)
                {
                    // 처리할 자료가 있을 경우 쿼리실행
                    this._common.ExecuteNonQuery(sqlCommand, ref sqlResult);

                    if (sqlResult.Success == true)
                    {
                        // 작업 성공시
                        if (sqlResult.AffectedRecords > 0)
                            this._common.MessageBox(MessageBoxIcon.Information, "자료를 삭제하였습니다." + Environment.NewLine +
                                string.Format("(삭제된 자료건 수 총 : {0}건)", sqlResult.AffectedRecords));
                        else
                            this._common.MessageBox(MessageBoxIcon.Information, "삭제된 자료가 없습니다.");
                    }
                    else
                        // 작업 실패시
                        MessageBox.Show(sqlResult.ErrorMsg);
                }
                else
                    // 처리할 자료가 없을 경우
                    this._common.MessageBox(MessageBoxIcon.Information, "삭제할 자료가 없습니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlCommand.Dispose();
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 캠퍼스 콩알관리자를 삭제한다.
        /// </summary>
        /// <history>
        /// 박석제, 2014-10-07, 생성
        /// </history>
        private void DeletePointManager()
        {
            Boolean isFound = false; // 처리할 자료가 있는지 체크할 변수
            DialogResult result = this._common.MessageBox(MessageBoxIcon.Question, "정말 삭제하시겠습니까?");
            if (result == DialogResult.No)
            {
                return;
            }

            SqlCommand sqlCommand = new SqlCommand();
            SqlResult sqlResult = new SqlResult();

            this.Cursor = Cursors.WaitCursor;

            try
            {
                // 컬럼 루프
                for (int rowCount = 0; rowCount <= dataGridViewPointManager.Rows.Count - 1; rowCount++)
                {
                    if (GetCellValue(dataGridViewPointManager, rowCount, "check_yn") == "1")
                    {
                        isFound = true;
                        sqlCommand.CommandText += @"
                            DELETE
                              FROM tls_point_manager 
                             WHERE cpno = '" + this._common.GetCellValue(dataGridViewPointManager, rowCount, "cpno") + @"'
                               AND userid = '" + this._common.GetCellValue(dataGridViewPointManager, rowCount, "userid") + @"' ";                               
                        Console.WriteLine(sqlCommand.CommandText);
                    }
                }

                if (isFound == true)
                {
                    // 처리할 자료가 있을 경우 쿼리실행
                    this._common.ExecuteNonQuery(sqlCommand, ref sqlResult);

                    if (sqlResult.Success == true)
                    {
                        // 작업 성공시
                        if (sqlResult.AffectedRecords > 0)
                            this._common.MessageBox(MessageBoxIcon.Information, "자료를 삭제하였습니다." + Environment.NewLine +
                                string.Format("(삭제 자료건 수 총 : {0}건)", sqlResult.AffectedRecords));
                        else
                            this._common.MessageBox(MessageBoxIcon.Information, "삭제된 자료가 없습니다.");
                    }
                    else
                        // 작업 실패시
                        MessageBox.Show(sqlResult.ErrorMsg);
                }
                else
                    // 처리할 자료가 없을 경우
                    this._common.MessageBox(MessageBoxIcon.Information, "삭제할 자료가 없습니다.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlCommand.Dispose();
                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        ///특정 캠퍼스그룹의 특정캠퍼스 배치 진행을 한다.
        /// </summary>
        /// <history>        
        /// </history>
        private void ImportCampus()
        {
            if (dataGridViewCampus.Rows.Count > 0 && dataGridViewCampus.CurrentCell != null)
            {                
                DialogResult result = this._common.MessageBox(MessageBoxIcon.Question, "캠퍼스 배치진행 하시겠습니까?");
                if (result == DialogResult.No)
                {
                    return;
                }

                SqlCommand sqlCommand = new SqlCommand();
                SqlResult sqlResult = new SqlResult();
                
                sqlCommand.CommandText += @"

                        DECLARE 
						@START_DATE		VARCHAR(8),
						@END_DATE		VARCHAR(8)

						SET @START_DATE = REPLACE(CONVERT(VARCHAR(10), '" + dateTimePickerImportCampusSdate.Value + @"', 112), '-', '')
						SET @END_DATE = REPLACE(CONVERT(VARCHAR(10), '" + dateTimePickerImportCampusEdate.Value + @"', 112), '-', '')
                           
                           EXEC S_D_IMPORT_U2M_CALL_CENTER_ALL '" + this._common.GetCellValue(dataGridViewCampus, dataGridViewCampus.CurrentCell.RowIndex, "cp_group_id") + @"'
                                                             , '" + GetCellValue(dataGridViewCampus, dataGridViewCampus.CurrentCell.RowIndex, "cpid") + @"'
                                                             , ''
                                                             , 'DAY'
                                                             , @START_DATE
                                                             , @END_DATE
                              
                ";
                Console.WriteLine(sqlCommand.CommandText);

                // 처리할 자료가 있을 경우 쿼리실행
                this._common.ExecuteNonQuery(sqlCommand, ref sqlResult);

                this._common.MessageBox(MessageBoxIcon.Information, "캠퍼스 배치진행 완료 하였습니다.");

            }
        }

        /// <summary>
        /// 미결사용자 정보를 등록한다
        /// </summary>
        /// <history>        
        /// </history>
        private void InsertByPass()
        {
            if (dataGridViewByPassUser.Rows.Count > 0 && dataGridViewByPassUser.CurrentCell != null)
            {
                DialogResult result = this._common.MessageBox(MessageBoxIcon.Question, "정말 등록 하시겠습니까?");
                if (result == DialogResult.No)
                {
                    return;
                }

                SqlCommand sqlCommand = new SqlCommand();
                SqlResult sqlResult = new SqlResult();

                sqlCommand.CommandText += @"
                            INSERT INTO [dbo].[TLS_BYPASS]
                                       ([CPNO]
                                       ,[CPID]
                                       ,[CPNM]
                                       ,[EMPNO]
                                       ,[EMPNM]
                                       ,[USERID]
                                       ,[USERNM]
                                       ,[USE_FROM]
                                       ,[USE_TO]
                                       ,[REASON]
                                       ,[COUNSEL_DATE]
                                       ,[COUNSEL_NM])
                                 VALUES
                                       ('" + GetCellValue(dataGridViewByPassUser, dataGridViewByPassUser.CurrentCell.RowIndex, "cpno") + @"'  
                                       , '" + GetCellValue(dataGridViewByPassUser, dataGridViewByPassUser.CurrentCell.RowIndex, "cpid") + @"'  
                                       , '" + GetCellValue(dataGridViewByPassUser, dataGridViewByPassUser.CurrentCell.RowIndex, "cpnm") + @"'  
                                       , '" + textBoxByPassEmpID.Text + @"'
                                       , '" + textBoxByPassEmpNM2.Text + @"'
                                       , '" + textBoxByPassUserID.Text + @"'
                                       , '" + textBoxByPassUserNM2.Text + @"'
                                       , REPLACE(CONVERT(VARCHAR(10), '" + dateTimePickerByPassUseFrom.Value + @"', 112), '-', '')
                                       , REPLACE(CONVERT(VARCHAR(10), '" + dateTimePickerByPassUseTo.Value + @"', 112), '-', '')
                                       ,'" + textBoxByPassReason.Text + @"'
                                       , getdate()
                                       , '관리자')
                             
                ";
                Console.WriteLine(sqlCommand.CommandText);

                // 처리할 자료가 있을 경우 쿼리실행
                this._common.ExecuteNonQuery(sqlCommand, ref sqlResult);

                this._common.MessageBox(MessageBoxIcon.Information, "자료를 등록 하였습니다.");

            }
        }

        /// <summary>
        /// 미결사용자 정보를 수정한다
        /// </summary>
        /// <history>        
        /// </history>
        private void UpdateByPass()
        {
            if (dataGridViewByPass.Rows.Count > 0 && dataGridViewByPass.CurrentCell != null)
            {                
                DialogResult result = this._common.MessageBox(MessageBoxIcon.Question, "정말 수정 하시겠습니까?");
                if (result == DialogResult.No)
                {
                    return;
                }

                SqlCommand sqlCommand = new SqlCommand();
                SqlResult sqlResult = new SqlResult();

                sqlCommand.CommandText += @"
                            UPDATE tls_bypass SET use_from = REPLACE(CONVERT(VARCHAR(10), '" + dateTimePickerByPassUseFrom.Value + @"', 112), '-', '')
                                                , use_to = REPLACE(CONVERT(VARCHAR(10), '" + dateTimePickerByPassUseTo.Value + @"', 112), '-', '')
                                                , reason = '" + textBoxByPassReason.Text + @"'
                             WHERE cpno = '" + this._common.GetCellValue(dataGridViewByPass, dataGridViewByPass.CurrentCell.RowIndex, "cpno") + @"' 
                               AND cpid = '" + GetCellValue(dataGridViewByPass, dataGridViewByPass.CurrentCell.RowIndex, "cpid") + @"'                              
                               AND request_seq = '" + GetCellValue(dataGridViewByPass, dataGridViewByPass.CurrentCell.RowIndex, "request_seq") + @"'
                               AND empnm = '" + GetCellValue(dataGridViewByPass, dataGridViewByPass.CurrentCell.RowIndex, "empnm") + @"'
                               AND empno = '" + GetCellValue(dataGridViewByPass, dataGridViewByPass.CurrentCell.RowIndex, "empno") + @"'
                               AND usernm = '" + GetCellValue(dataGridViewByPass, dataGridViewByPass.CurrentCell.RowIndex, "usernm") + @"'
                               AND userid = '" + GetCellValue(dataGridViewByPass, dataGridViewByPass.CurrentCell.RowIndex, "userid") + @"'
                               AND  STUFF(STUFF(use_from, 5, 0, '-'), 8, 0, '-') = '" + this._common.GetCellValue(dataGridViewByPass, dataGridViewByPass.CurrentCell.RowIndex, "use_from") + @"'
                               AND  STUFF(STUFF(use_to, 5, 0, '-'), 8, 0, '-') = '" + this._common.GetCellValue(dataGridViewByPass, dataGridViewByPass.CurrentCell.RowIndex, "use_to") + @"'
                ";
                Console.WriteLine(sqlCommand.CommandText);

                // 처리할 자료가 있을 경우 쿼리실행
                this._common.ExecuteNonQuery(sqlCommand, ref sqlResult);

                this._common.MessageBox(MessageBoxIcon.Information, "자료를 수정 하였습니다.");

            }
        }

        /// <summary>
        /// 미결사용자 정보를 삭제한다
        /// </summary>
        /// <history>        
        /// </history>
        private void DeleteByPass()
        {
            if (dataGridViewByPass.Rows.Count > 0 && dataGridViewByPass.CurrentCell != null)
            {
                DialogResult result = this._common.MessageBox(MessageBoxIcon.Question, "정말 삭제 하시겠습니까?");
                if (result == DialogResult.No)
                {
                    return;
                }

                SqlCommand sqlCommand = new SqlCommand();
                SqlResult sqlResult = new SqlResult();

                sqlCommand.CommandText += @"
                            DELETE 
                              FROM tls_bypass
                             WHERE cpno = '" + this._common.GetCellValue(dataGridViewByPass, dataGridViewByPass.CurrentCell.RowIndex, "cpno") + @"' 
                               AND cpid = '" + GetCellValue(dataGridViewByPass, dataGridViewByPass.CurrentCell.RowIndex, "cpid") + @"'                              
                               AND request_seq = '" + GetCellValue(dataGridViewByPass, dataGridViewByPass.CurrentCell.RowIndex, "request_seq") + @"'
                               AND empnm = '" + GetCellValue(dataGridViewByPass, dataGridViewByPass.CurrentCell.RowIndex, "empnm") + @"'
                               AND empno = '" + GetCellValue(dataGridViewByPass, dataGridViewByPass.CurrentCell.RowIndex, "empno") + @"'
                               AND usernm = '" + GetCellValue(dataGridViewByPass, dataGridViewByPass.CurrentCell.RowIndex, "usernm") + @"'
                               AND userid = '" + GetCellValue(dataGridViewByPass, dataGridViewByPass.CurrentCell.RowIndex, "userid") + @"'
                               AND STUFF(STUFF(use_from, 5, 0, '-'), 8, 0, '-') = '" + this._common.GetCellValue(dataGridViewByPass, dataGridViewByPass.CurrentCell.RowIndex, "use_from") + @"'
                               AND STUFF(STUFF(use_to, 5, 0, '-'), 8, 0, '-') = '" + this._common.GetCellValue(dataGridViewByPass, dataGridViewByPass.CurrentCell.RowIndex, "use_to") + @"'
                ";
                Console.WriteLine(sqlCommand.CommandText);

                // 처리할 자료가 있을 경우 쿼리실행
                this._common.ExecuteNonQuery(sqlCommand, ref sqlResult);

                this._common.MessageBox(MessageBoxIcon.Information, "자료를 삭제 하였습니다.");

            }
        }

        #endregion Method

        #region Event

        /// <summary>
        /// 폼 로드
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <history>
        /// 박석제, 2014-09-24, 생성
        /// </history>
        private void FormMain_Load(object sender, EventArgs e)
        {
            InitCombo();
            SelectDataGridView(dataGridViewCampus, "select_campus");

            _userControlStudy = new UserControlStudy();
            splitContainer9.Panel2.Controls.Add(_userControlStudy);
            
            //tabPage2.Controls.Add(userControlStudy);
            _userControlStudy.Visible = true;
            //userControlStudy.Select(this.StudyType);
            _userControlStudy.Select(this.StudyType, this.ClassEmployeeCPNO, this.ClassEmployeeCLNO, this.ClassStudentCPNO, this.ClassStudentUID, this.ClassEmployeeUID, this.ClassSchoolCDStudy);
        }

        private void dataGridViewClassStudent_MouseClick(object sender, MouseEventArgs e)
        {
            //메인화면 학생 u2m학습창 및 마이페이지 로그인
            if (e.Button == MouseButtons.Right)
            {
                int currentMouseOverRow = ((DataGridView)sender).HitTest(e.X, e.Y).RowIndex;
                if (currentMouseOverRow >= 0)
                {
                    ((DataGridView)sender).CurrentCell = ((DataGridView)sender)[0, currentMouseOverRow];
                    this._common.RunLogin(((DataGridView)sender), new Point(e.X, e.Y));
                }
            }
        }
        private void dataGridViewEduStudent_MouseClick(object sender, MouseEventArgs e)
        {
            //메인화면 드림+ 학생 u2m학습창 및 마이페이지 로그인
            if (e.Button == MouseButtons.Right)
            {
                int currentMouseOverRow = ((DataGridView)sender).HitTest(e.X, e.Y).RowIndex;
                if (currentMouseOverRow >= 0)
                {
                    ((DataGridView)sender).CurrentCell = ((DataGridView)sender)[0, currentMouseOverRow];
                    this._common.RunLogin(((DataGridView)sender), new Point(e.X, e.Y));
                }
            }
        }
        private void dataGridViewStudent_MouseClick(object sender, MouseEventArgs e)
        {
            //차시관리 학생 u2m학습창 및 마이페이지 로그인
            if (e.Button == MouseButtons.Right)
            {
                int currentMouseOverRow = ((DataGridView)sender).HitTest(e.X, e.Y).RowIndex;
                if (currentMouseOverRow >= 0)
                {
                    ((DataGridView)sender).CurrentCell = ((DataGridView)sender)[0, currentMouseOverRow];
                    this._common.RunLogin(((DataGridView)sender), new Point(e.X, e.Y));
                }
            }
        }

        private void dataGridViewByPass_MouseClick(object sender, MouseEventArgs e)
        {
            //미결사용 등록 u2m학습창 및 마이페이지 로그인
            if (e.Button == MouseButtons.Right)
            {
                int currentMouseOverRow = ((DataGridView)sender).HitTest(e.X, e.Y).RowIndex;
                if (currentMouseOverRow >= 0)
                {
                    ((DataGridView)sender).CurrentCell = ((DataGridView)sender)[0, currentMouseOverRow];
                    this._common.RunLogin(((DataGridView)sender), new Point(e.X, e.Y));
                }
            }
            if (e.Button == MouseButtons.Left)
            {
                if (dataGridViewByPass.Rows.Count > 0 && dataGridViewByPass.CurrentCell != null)
                {
                    //미경등록자 정보를 대상자명 신청자명, Emp_ID, UserID 텍스트박스에 표시한다.
                    textBoxByPassEmpNM2.Text = GetCellValue(dataGridViewByPass, dataGridViewByPass.CurrentCell.RowIndex, "empnm");
                    textBoxByPassEmpID.Text = GetCellValue(dataGridViewByPass, dataGridViewByPass.CurrentCell.RowIndex, "empno");
                    textBoxByPassUserNM2.Text = GetCellValue(dataGridViewByPass, dataGridViewByPass.CurrentCell.RowIndex, "usernm");
                    textBoxByPassUserID.Text = GetCellValue(dataGridViewByPass, dataGridViewByPass.CurrentCell.RowIndex, "userid");
                    this.dateTimePickerByPassUseFrom.Value = DateTime.Parse(this._common.GetCellValue(dataGridViewByPass, dataGridViewByPass.CurrentCell.RowIndex, "use_from"));
                    this.dateTimePickerByPassUseTo.Value = DateTime.Parse(this._common.GetCellValue(dataGridViewByPass, dataGridViewByPass.CurrentCell.RowIndex, "use_to"));
                    textBoxByPassReason.Text = GetCellValue(dataGridViewByPass, dataGridViewByPass.CurrentCell.RowIndex, "reason");
                }
            }
        }

        private void dataGridViewByPassUser_MouseClick(object sender, MouseEventArgs e)
        {
            //미결사용 대상자 u2m학습창 및 마이페이지 로그인
            if (e.Button == MouseButtons.Right)
            {
                int currentMouseOverRow = ((DataGridView)sender).HitTest(e.X, e.Y).RowIndex;
                if (currentMouseOverRow >= 0)
                {
                    ((DataGridView)sender).CurrentCell = ((DataGridView)sender)[0, currentMouseOverRow];
                    this._common.RunLogin(((DataGridView)sender), new Point(e.X, e.Y));
                }
            }

        }

        private void dataGridViewStudentPoint_MouseClick(object sender, MouseEventArgs e)
        {
            //콩알관리 학생 u2m학습창 및 마이페이지 로그인
            if (e.Button == MouseButtons.Right)
            {
                int currentMouseOverRow = ((DataGridView)sender).HitTest(e.X, e.Y).RowIndex;
                if (currentMouseOverRow >= 0)
                {
                    ((DataGridView)sender).CurrentCell = ((DataGridView)sender)[0, currentMouseOverRow];
                    this._common.RunLogin(((DataGridView)sender), new Point(e.X, e.Y));
                }
            }
            if (e.Button == MouseButtons.Left)
            {
                if (dataGridViewStudentPoint.Rows.Count > 0 && dataGridViewStudentPoint.CurrentCell != null)
                {
                    //학생 콩알 내역 조회
                    if (dataGridViewStudentPoint.Rows.Count > 0 && dataGridViewStudentPoint.CurrentCell != null)
                    {
                        SelectDataGridView(dataGridViewStudentPointSave, "select_student_point_save");                        
                    }
                }
            }
        }

        private void dataGridViewClassPoint_KeyDown(object sender, KeyEventArgs e)
        {
            //콩알관리 반 선택 Ctrl + 1, 2, 3 체크박스 선택
            if (e.Control && (e.KeyCode == Keys.D1 || e.KeyCode == Keys.D2 || e.KeyCode == Keys.D3))
                _common.GridCheck((DataGridView)sender, e);
        }
        private void dataGridViewStudentPointSave_KeyDown(object sender, KeyEventArgs e)
        {
            //콩알관리 학생 콩알내역 선택 Ctrl + 1, 2, 3 체크박스 선택
            if (e.Control && (e.KeyCode == Keys.D1 || e.KeyCode == Keys.D2 || e.KeyCode == Keys.D3))
                _common.GridCheck((DataGridView)sender, e);
        }
        private void dataGridViewPointManager_KeyDown(object sender, KeyEventArgs e)
        {
            //콩알관리 캠퍼스 콩알관리자 선택 Ctrl + 1, 2, 3 체크박스 선택
            if (e.Control && (e.KeyCode == Keys.D1 || e.KeyCode == Keys.D2 || e.KeyCode == Keys.D3))
                _common.GridCheck((DataGridView)sender, e);
        }

        /// <summary>
        ///  드림플러스 특정 캠퍼스 정보를 유투엠에 연동한다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonImportCampusInfo_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(toolStripTextBoxCampusInfo.Text))
            {
                ImportCampus();
                this.dateTimePickerImportCampusSdate.Value = DateTime.Now;
                this.dateTimePickerImportCampusEdate.Value = DateTime.Now;
            }
            
        }

        /// <summary>
        ///  드림플러스 학생 정보를 유투엠에 연동한다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonImportStudentInfo_Click(object sender, EventArgs e)
        {
            if (dataGridViewEduStudent.Rows.Count > 0 && dataGridViewEduStudent.CurrentCell != null)
            {
                if (this._common.MessageBox(MessageBoxIcon.Question, "배치를 실행하시겠습니까?") == System.Windows.Forms.DialogResult.No) return;

                this.Cursor = Cursors.WaitCursor;

                Common.ParametersForImport paramsForImport = new Common.ParametersForImport();
                paramsForImport.AcadGroupId = GetCellValue(dataGridViewCampus, dataGridViewCampus.CurrentCell.RowIndex, "cp_group_id"); ;
                paramsForImport.AcadId = GetCellValue(dataGridViewCampus, dataGridViewCampus.CurrentCell.RowIndex, "cpid"); ;
                paramsForImport.ClassId = "";
                paramsForImport.StudentId = GetCellValue(dataGridViewEduStudent, dataGridViewEduStudent.CurrentCell.RowIndex, "student_id"); ;
                paramsForImport.StartDate = "";
                paramsForImport.EndDate = "";

                this._common.ImportDreamPlusStudentInfoToU2M(ref paramsForImport);

                if (paramsForImport.SuccessYn == "N")
                    this._common.MessageBox(MessageBoxIcon.Error, paramsForImport.ErrorMessage);
                else
                    this._common.MessageBox(MessageBoxIcon.Information, "배치가 완료되었습니다.");

                this.Cursor = Cursors.Default;
            }
        }

        /// <summary>
        ///  드림플러스 강사 비번 정보를 유투엠에 연동한다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonEmployeeLoginPW_Click(object sender, EventArgs e)
        {
            if (dataGridViewEmployee.Rows.Count > 0 && dataGridViewEmployee.CurrentCell != null)
            {
                if (this._common.MessageBox(MessageBoxIcon.Question, "비번 동기화를 실행하시겠습니까?") == System.Windows.Forms.DialogResult.No) return;

                this.Cursor = Cursors.WaitCursor;

                Common.ParametersForImport paramsForImport = new Common.ParametersForImport();
                paramsForImport.UserId = GetCellValue(dataGridViewEmployee, dataGridViewEmployee.CurrentCell.RowIndex, "userid"); ;

                this._common.SyncDreamPlusPasswordToU2M(ref paramsForImport);

                if (paramsForImport.SuccessYn == "N")
                    this._common.MessageBox(MessageBoxIcon.Error, paramsForImport.ErrorMessage);
                else
                    this._common.MessageBox(MessageBoxIcon.Information, "비번 동기화가 완료되었습니다.");

                this.Cursor = Cursors.Default;

            }
        }


        /// <summary>
        ///  드림플러스 학생 비번 정보를 유투엠에 연동한다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonStudentLoginPW_Click(object sender, EventArgs e)
        {
            if (dataGridViewClassStudent.Rows.Count > 0 && dataGridViewClassStudent.CurrentCell != null)
            {
                if (this._common.MessageBox(MessageBoxIcon.Question, "비번 동기화를 실행하시겠습니까?") == System.Windows.Forms.DialogResult.No) return;

                this.Cursor = Cursors.WaitCursor;

                Common.ParametersForImport paramsForImport = new Common.ParametersForImport();
                paramsForImport.UserId = GetCellValue(dataGridViewClassStudent, dataGridViewClassStudent.CurrentCell.RowIndex, "userid"); ;

                this._common.SyncDreamPlusPasswordToU2M(ref paramsForImport);

                if (paramsForImport.SuccessYn == "N")
                    this._common.MessageBox(MessageBoxIcon.Error, paramsForImport.ErrorMessage);
                else
                    this._common.MessageBox(MessageBoxIcon.Information, "비번 동기화가 완료되었습니다.");

                this.Cursor = Cursors.Default;

            }
        }

        /// <summary>
        /// 캠퍼스 구분 콤보박스 선택 변경시 발생하는 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <history>
        /// 박석제, 2014-09-24, 생성
        /// </history>
        private void comboBoxCampusType_SelectionChangeCommitted(object sender, EventArgs e)
        {
            // 캠퍼스 콤보박스 데이터 생성
            string campusType = comboBoxCampusType.SelectedValue.ToString().Trim();

            _common.GetComboList(comboBoxCampus, "캠퍼스", true, new string[] { campusType });
            SelectDataGridView(dataGridViewCampus, "select_campus");
        }
        
        /// <summary>
        /// 캠퍼스 콤보박스 선택 변경시 발생하는 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <history>
        /// 박석제, 2014-09-24, 생성
        /// </history>
        private void comboBoxCampus_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SelectDataGridView(dataGridViewCampus, "select_campus");
        }

        private void textBoxCampus_KeyDown(object sender, KeyEventArgs e)
        {
            //특정 캠퍼스 조회
            if (e.KeyCode == Keys.Enter)
            {
                SelectDataGridView(dataGridViewCampus, "select_campus");                
            }
        }
        /// <summary>
        /// 캠퍼스 목록 클릭시 발생하는 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <history>
        /// 박석제, 2014-09-24, 생성
        /// </history>
        private void dataGridViewCampus_Click(object sender, EventArgs e)
        {
            // 교사정보를 조회한다.
            SelectDataGridView(dataGridViewEmployee, "select_employee");
            toolStripTextBoxCampusInfo.Text = GetCellValue(dataGridViewCampus, dataGridViewCampus.CurrentCell.RowIndex, "cpnm");
        }

        /// <summary>
        /// 교사정보 클릭시 발생하는 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewEmployee_Click(object sender, EventArgs e)
        {
            if (dataGridViewEmployee.Rows.Count > 0 && dataGridViewEmployee.CurrentCell != null)
            {
                // 수업교사 반 목록을 조회한다.
                SelectDataGridView(dataGridViewClassEmployee, "select_class_employee");
                // userid, login_id, login_pw 텍스트박스에서 표시한다.            
                textBoxUserID.Text = GetCellValue(dataGridViewEmployee, dataGridViewEmployee.CurrentCell.RowIndex, "userid");
                textBoxMemberID.Text = GetCellValue(dataGridViewEmployee, dataGridViewEmployee.CurrentCell.RowIndex, "login_id");
                textBoxLoginPW.Text = GetCellValue(dataGridViewEmployee, dataGridViewEmployee.CurrentCell.RowIndex, "login_pwd");
                
            }
        }

        private void textBoxClassNM_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (dataGridViewEmployee.Rows.Count > 0 && dataGridViewEmployee.CurrentCell != null)
                {
                    //특정 반 목록 조회
                    SelectDataGridView(dataGridViewClassEmployee, "select_class_employee_all");
                    textBoxClassNM.Text = "";
                }
                
            }
            
        }
        /// <summary>
        /// 직원명 검색 TextBox 에 Enter 키 입력시 발생하는 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <history>
        /// 박석제, 2014-09-24, 생성
        /// </history>
        private void textBoxUserNm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //직원을 검색한다.                
                SelectDataGridView(dataGridViewEmployee, "select_employee_all");                
                textBoxUserID.Text = "";
                textBoxMemberID.Text = "";
                textBoxLoginPW.Text = "";
            }
        }

        /// <summary>
        /// 반에 배정된 학생을 조회한다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <history>
        /// 박석제, 2014-10-08, check_yn 컬럼 클릭시 조회안되도록 수정
        /// </history>
        private void dataGridViewClassEmployee_Click(object sender, EventArgs e)
        {
            if (dataGridViewClassEmployee.Rows.Count > 0 && dataGridViewClassEmployee.CurrentCell != null)
            {
                //반 학생 목록을 조회한다.
                SelectDataGridView(dataGridViewClassStudent, "select_class_student");
                textBoxClassNM.Text = this._common.GetCellValue(dataGridViewClassEmployee, dataGridViewClassEmployee.CurrentCell.RowIndex, "clnm");
            }
        }

        private void textBoxStudentNM_KeyDown(object sender, KeyEventArgs e)
        {            
            if (e.KeyCode == Keys.Enter)
            {
                if (dataGridViewEmployee.Rows.Count > 0 && dataGridViewEmployee.CurrentCell != null)
                {
                    //메인화면 캠퍼스별 특정 학생을 검색한다.
                    SelectDataGridView(dataGridViewClassStudent, "select_class_student_all");
                }
                
            }
        }
        private void textBoxEduStudentNM_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //드림+ 학생을 조회한다
                SelectDataGridView(dataGridViewEduStudent, "select_class_edu_student_all");
            }
        }
        private void toolStripButtonSelect_Student_Click(object sender, EventArgs e)
        {
            //폼2 이동 (학생검색)
            FormStudent frm2 = new FormStudent();
            frm2.Show();
        }

        /// <summary>
        /// 반에 배정된 차시를 조회한다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <history>
        /// 박석제, 2014-10-08, check_yn 컬럼 클릭시 조회안되도록 수정
        /// </history>
        private void dataGridViewCampus_DoubleClick(object sender, EventArgs e)
        {
            //캠퍼스 더블클릭 시 반(과정1) 차시폼 조회 이동            
            if (dataGridViewCampus.CurrentCell != null)
            {
                FormClassStudentSchedule classStudentSchedule = new FormClassStudentSchedule();
                classStudentSchedule.ClassEmployeeCPNO = this._common.GetCellValue(dataGridViewCampus, dataGridViewCampus.CurrentCell.RowIndex, "cpno");
                //classStudentSchedule.ClassStudentCPNO = this._common.GetCellValue(dataGridViewCampus, dataGridViewCampus.CurrentCell.RowIndex, "cpno");
                //classStudentSchedule.StudyType = "S";
                classStudentSchedule.StudyType = "C";
                classStudentSchedule.Show();
            }
        }

        private void dataGridViewEmployee_DoubleClick(object sender, EventArgs e)
        {
            //수업교사 더블클릭 시 반(과정1) 차시폼 조회 이동
            if (dataGridViewEmployee.CurrentCell != null)
            {
                FormClassStudentSchedule classStudentSchedule = new FormClassStudentSchedule();
                classStudentSchedule.ClassEmployeeCPNO = this._common.GetCellValue(dataGridViewEmployee, dataGridViewEmployee.CurrentCell.RowIndex, "cpno");
                //classStudentSchedule.ClassStudentCPNO = this._common.GetCellValue(dataGridViewEmployee, dataGridViewEmployee.CurrentCell.RowIndex, "cpno");
                classStudentSchedule.ClassEmployeeUID = this._common.GetCellValue(dataGridViewEmployee, dataGridViewEmployee.CurrentCell.RowIndex, "userid");
                //classStudentSchedule.StudyType = "S";
                classStudentSchedule.StudyType = "C";
                classStudentSchedule.Show();
            }
        }

        private void dataGridViewClassEmployee_DoubleClick(object sender, EventArgs e)
        {
            //반 차시 조회 폼 이동
            if (dataGridViewClassEmployee.Rows.Count > 0 && dataGridViewClassEmployee.CurrentCell != null)
            {
                if (dataGridViewClassEmployee.Columns[dataGridViewClassEmployee.CurrentCell.ColumnIndex].DataPropertyName != "check_yn")
                {
                    FormClassStudentSchedule classStudentSchedule = new FormClassStudentSchedule();
                    classStudentSchedule.StudyType = "C";
                    classStudentSchedule.ClassEmployeeCPNO = this._common.GetCellValue(dataGridViewClassEmployee, dataGridViewClassEmployee.CurrentCell.RowIndex, "cpno");
                    classStudentSchedule.ClassEmployeeCLNO = this._common.GetCellValue(dataGridViewClassEmployee, dataGridViewClassEmployee.CurrentCell.RowIndex, "clno");
                    classStudentSchedule.Show();
                }
            }                        
        }

        private void dataGridViewClassStudent_DoubleClick(object sender, EventArgs e)
        {
            //학생 차시 조회 폼 이동
            if (dataGridViewClassStudent.Rows.Count > 0 && dataGridViewClassStudent.CurrentCell != null)
            {
                FormClassStudentSchedule classStudentSchedule = new FormClassStudentSchedule();
                classStudentSchedule.ClassEmployeeCPNO = this._common.GetCellValue(dataGridViewClassStudent, dataGridViewClassStudent.CurrentCell.RowIndex, "cpno");
                classStudentSchedule.ClassStudentUID = this._common.GetCellValue(dataGridViewClassStudent, dataGridViewClassStudent.CurrentCell.RowIndex, "userid");
                classStudentSchedule.StudyType = "S";
                classStudentSchedule.Show();
            }
            
        }

        private void toolStripButtonClassStudy_Click(object sender, EventArgs e)
        {
            //반 차시 조회 폼 이동
            if (dataGridViewCampus.CurrentCell != null)
            {
                FormClassStudentSchedule classStudentSchedule = new FormClassStudentSchedule();
                classStudentSchedule.ClassEmployeeCPNO = this._common.GetCellValue(dataGridViewCampus, dataGridViewCampus.CurrentCell.RowIndex, "cpno");
                classStudentSchedule.StudyType = "C";
                classStudentSchedule.Show();

            }
            
        }

        private void toolStripButtonStudentStudy_Click(object sender, EventArgs e)
        {
            //학생 차시 조회 폼 이동
            FormClassStudentSchedule classStudentSchedule = new FormClassStudentSchedule();
            classStudentSchedule.ClassEmployeeCPNO = this._common.GetCellValue(dataGridViewCampus, dataGridViewCampus.CurrentCell.RowIndex, "cpno");
            classStudentSchedule.StudyType = "S";
            classStudentSchedule.Show();
            
        }

        /// <summary>
        /// 캠퍼스 구분(포인트) 콤보박스 선택 변경시 발생하는 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <history>
        /// 박석제, 2014-09-24, 생성
        /// </history>
        private void comboBoxCampusTypePoint_SelectionChangeCommitted(object sender, EventArgs e)
        {
            // 캠퍼스 콤보박스 데이터 생성
            string campusType = comboBoxCampusTypePoint.SelectedValue.ToString().Trim();

            _common.GetComboList(comboBoxCampusPoint, "캠퍼스", true, new string[] { campusType });
            SelectDataGridView(dataGridViewCampusPoint, "select_campus_point");
        }

        /// <summary>
        /// 캠퍼스(포인트) 콤보박스 선택 변경시 발생하는 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <history>
        /// 박석제, 2014-09-24, 생성
        /// </history>
        private void comboBoxCampusPoint_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SelectDataGridView(dataGridViewCampusPoint, "select_campus_point");
        }

        /// <summary>
        /// 학교급(포인트) 목록 클릭시 발생하는 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <history>
        /// 박석제, 2014-09-24, 생성
        /// </history>
        private void comboBoxSchoolCDPoint_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (dataGridViewCampusPoint.Rows.Count > 0 && dataGridViewCampusPoint.CurrentCell != null)
            {
                SelectDataGridView(dataGridViewClassPoint, "select_class_point");
            }
        }


        /// <summary>
        /// 캠퍼스(포인트) 목록 클릭시 발생하는 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <history>
        /// 박석제, 2014-09-24, 생성하였음.
        /// </history>
        private void dataGridViewCampusPoint_Click(object sender, EventArgs e)
        {
            //반 콩알정보 조회한다
            if (dataGridViewCampusPoint.Rows.Count > 0 && dataGridViewCampusPoint.CurrentCell != null)
            {
                SelectDataGridView(dataGridViewClassPoint, "select_class_point");
            }
        }

        private void buttonSelectNewClass_Click(object sender, EventArgs e)
        {
            //신규 반 조회
            if (dataGridViewCampusPoint.Rows.Count > 0 && dataGridViewCampusPoint.CurrentCell != null)
            {
                SelectDataGridView(dataGridViewClassPoint, "select_new_class_point");
            }
        }

        private void buttonNewClassPoint_Click(object sender, EventArgs e)
        {
            //신규 반 콩알 적립 (적립일수 * 학생수 * 20)
            UpdateClassPoint();
            textBoxPoint.Text = "";
            if (dataGridViewClassPoint.Rows.Count > 0 && dataGridViewClassPoint.CurrentCell != null)
            {
                SelectDataGridView(dataGridViewClassPoint, "select_class_point");
            }
        }

        private void dataGridViewClassPoint_Click(object sender, EventArgs e)
        {
            //학생 콩알정보 조회
            if (dataGridViewClassPoint.Rows.Count > 0 && dataGridViewClassPoint.CurrentCell != null)
            {
                SelectDataGridView(dataGridViewStudentPoint, "select_student_point");
            }
        }

        private void textBoxStudentNMPoint_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //학생콩알정보 조회
                SelectDataGridView(dataGridViewStudentPoint, "select_student_point_all");
                //dataGridViewCampusPoint.Rows.Clear();
                //dataGridViewClassPoint.Rows.Clear();
            }
        }

        private void buttonInsertStudentPoint_Click(object sender, EventArgs e)
        {            
            ////학생 콩알 적립
            SelectDataGridView(dataGridViewStudentPointSave, "insert_student_point");
            SelectDataGridView(dataGridViewStudentPointSave, "select_student_point_save");
        }
        private void buttonDeleteStudentPoint_Click(object sender, EventArgs e)
        {
            //학생 콩알 내역 삭제
            DeleteStudentPoint();
            SelectDataGridView(dataGridViewStudentPointSave, "select_student_point_save");
        }

        private void buttonSelectPointManager_Click(object sender, EventArgs e)
        {
            //캠퍼스 콩알 관리자 조회
            SelectDataGridView(dataGridViewPointManager, "select_point_manager");
            textBoxPointManagerCpno.Text = "CPNO";
            textBoxPointManagerUserid.Text = "USERID";
        }

        private void buttonInsertPointManager_Click(object sender, EventArgs e)
        {
            //캠퍼스 콩알 관리자 등록
            SelectDataGridView(dataGridViewPointManager, "insert_point_manager");
        }
        private void buttonDeletePointManager_Click(object sender, EventArgs e)
        {
            //캠퍼스 콩알관리자 삭제
            DeletePointManager();
            SelectDataGridView(dataGridViewPointManager, "select_point_manager");
        }

        private void comboBoxCampusTypeStudy_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //차시관리 캠퍼스 콤보박스 데이터 생성
            string campusTypeStudy = comboBoxCampusTypeStudy.SelectedValue.ToString().Trim();

            _common.GetComboList(comboBoxCampusStudy, "캠퍼스", true, new string[] { campusTypeStudy });            
        }

        private void comboBoxCampusStudy_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //차시관리 캠퍼스 콤보박스 데이터 조회            
            SelectDataGridView(dataGridViewClass, "select_class");            
            this.StudyType = "N";            
            //this.ClassEmployeeCPNO = this._common.GetCellValue(dataGridViewClass, dataGridViewClass.CurrentCell.RowIndex, "cpno");            
            _userControlStudy.Select(this.StudyType, this.ClassEmployeeCPNO, this.ClassEmployeeCLNO, this.ClassStudentCPNO, this.ClassStudentUID, this.ClassEmployeeUID, this.ClassSchoolCDStudy);                        
        }

        private void comboBoxYyyyStudy_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //차시관리 분기 콤보박스 데이터 생성 (차시관리 년도)
            string campusStudy = comboBoxCampusStudy.SelectedValue.ToString();
            string yyyyStudy = comboBoxYyyyStudy.SelectedValue.ToString();
            _common.GetComboList(comboBoxTermCDStudy, "분기", true, new string[] { campusStudy, yyyyStudy });
        }

        private void comboBoxSchoolCDStudy_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //차시관리 분기 콤보박스 데이터 생성 (차시관리 학교급)
            string campusStudy = comboBoxCampusStudy.SelectedValue.ToString();
            string yyyyStudy = comboBoxYyyyStudy.SelectedValue.ToString();
            string schoolCDStudy = comboBoxSchoolCDStudy.SelectedValue.ToString();
            _common.GetComboList(comboBoxTermCDStudy, "분기", true, new string[] { campusStudy, yyyyStudy, schoolCDStudy });
            SelectDataGridView(dataGridViewClass, "select_class");
            //this.StudyType = "N";            
            //this.ClassEmployeeCPNO = this._common.GetCellValue(dataGridViewClass, dataGridViewClass.CurrentCell.RowIndex, "cpno");
            //this.ClassSchoolCDStudy = this._common.GetCellValue(dataGridViewClass, dataGridViewClass.CurrentCell.RowIndex, "school_cd");
            //_userControlStudy.Select(this.StudyType, this.ClassEmployeeCPNO, this.ClassEmployeeCLNO, this.ClassStudentCPNO, this.ClassStudentUID, this.ClassEmployeeUID, this.ClassSchoolCDStudy);
        }

        private void textBoxClassStudy_KeyDown(object sender, KeyEventArgs e)
        {
            //차시관리 특정 반 조회
            if (e.KeyCode == Keys.Enter)
            {
                SelectDataGridView(dataGridViewClass, "select_class");
            }
        }
        private void textBoxStudentStudy_KeyDown(object sender, KeyEventArgs e)
        {
            //차시관리 특정 학생 조회
            if (e.KeyCode == Keys.Enter)
            {
                SelectDataGridView(dataGridViewStudent, "select_student_all");
            }
        }
        private void buttonSelectStudy_Click(object sender, EventArgs e)
        {
            //차시관리 반 목록 조회            
            SelectDataGridView(dataGridViewClass, "select_class");            
        }

        private void textBoxCampusStudy_KeyDown(object sender, KeyEventArgs e)
        {
            //차시관리 특정 캠퍼스의 반 목록를 조회한다
            if (e.KeyCode == Keys.Enter)
            {
                SelectDataGridView(dataGridViewClass, "select_class");
            }
            
        }
        private void dataGridViewClass_Click(object sender, EventArgs e)
        {
            
            //차시관리 반 학생 조회
            if (dataGridViewClass.Rows.Count > 0 && dataGridViewClass.CurrentCell != null)
            {
                SelectDataGridView(dataGridViewStudent, "select_student");                
            }
            
        }
        private void dataGridViewClass_DoubleClick(object sender, EventArgs e)
        {
            //차시관리 반 차시 조회
            if (dataGridViewClass.Rows.Count > 0 && dataGridViewClass.CurrentCell != null)
            {
                this.StudyType = "C";
                this.ClassEmployeeCPNO = this._common.GetCellValue(dataGridViewClass, dataGridViewClass.CurrentCell.RowIndex, "cpno");
                this.ClassEmployeeCLNO = this._common.GetCellValue(dataGridViewClass, dataGridViewClass.CurrentCell.RowIndex, "clno");
                _userControlStudy.Select(this.StudyType, this.ClassEmployeeCPNO, this.ClassEmployeeCLNO, this.ClassStudentCPNO, this.ClassStudentUID, this.ClassEmployeeUID, this.ClassSchoolCDStudy);
            }            
        }
        private void dataGridViewStudent_DoubleClick(object sender, EventArgs e)
        {
            //차시관리 학생 차시 조회
            if (dataGridViewStudent.Rows.Count > 0 && dataGridViewStudent.CurrentCell != null)
            {
                this.StudyType = "S";
                this.ClassStudentCPNO = this._common.GetCellValue(dataGridViewStudent, dataGridViewStudent.CurrentCell.RowIndex, "cpno");
                //this.ClassEmployeeCLNO = this._common.GetCellValue(dataGridViewStudent, dataGridViewStudent.CurrentCell.RowIndex, "clno");
                this.ClassStudentUID = this._common.GetCellValue(dataGridViewStudent, dataGridViewStudent.CurrentCell.RowIndex, "userid");
                _userControlStudy.Select(this.StudyType, this.ClassEmployeeCPNO, this.ClassEmployeeCLNO, this.ClassStudentCPNO, this.ClassStudentUID, this.ClassEmployeeUID, this.ClassSchoolCDStudy);
            }
        }

        private void comboBoxCampusTypeByPass_SelectionChangeCommitted(object sender, EventArgs e)
        {
            // 미결사용자 등록 캠퍼스 콤보박스 데이터 생성
            string campusType = comboBoxCampusTypeByPass.SelectedValue.ToString().Trim();
            _common.GetComboList(comboBoxCampusByPass, "캠퍼스", true, new string[] { campusType });
        }
        private void textBoxStudentNmByPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //미결사용 학생 검색
                SelectDataGridView(dataGridViewByPass, "select_bypass");
            }
        }

        private void buttonSelectByPass_Click(object sender, EventArgs e)
        {
            //미결사용자 검색
            SelectDataGridView(dataGridViewByPass, "select_bypass");
        }

        private void textBoxByPassEmpNM_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //미결사용등록 신청자명을 조회한다.
                SelectDataGridView(dataGridViewByPassEmp, "select_bypass_emp");
                textBoxByPassEmpNM.Text = "";
            }
        }

        private void textBoxByPassUserNM_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //미결사용등록 대상자명을 조회한다.
                SelectDataGridView(dataGridViewByPassUser, "select_bypass_user");
                textBoxByPassUserNM.Text = "";
            }
        }
        

        private void dataGridViewByPassEmp_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridViewByPassEmp.Rows.Count > 0 && dataGridViewByPassEmp.CurrentCell != null)
            {                
                //신청자명, Emp_id 텍스트박스에 표시한다.            
                textBoxByPassEmpNM2.Text = GetCellValue(dataGridViewByPassEmp, dataGridViewByPassEmp.CurrentCell.RowIndex, "usernm");
                textBoxByPassEmpID.Text = GetCellValue(dataGridViewByPassEmp, dataGridViewByPassEmp.CurrentCell.RowIndex, "userid");
            }
        }

        private void dataGridViewByPassUser_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridViewByPassUser.Rows.Count > 0 && dataGridViewByPassUser.CurrentCell != null)
            {
                //대상자명, UserID 텍스트박스에 표시한다.            
                textBoxByPassUserNM2.Text = GetCellValue(dataGridViewByPassUser, dataGridViewByPassUser.CurrentCell.RowIndex, "usernm");
                textBoxByPassUserID.Text = GetCellValue(dataGridViewByPassUser, dataGridViewByPassUser.CurrentCell.RowIndex, "userid");
            }
        }

        

        private void buttonByPassReset_Click(object sender, EventArgs e)
        {
            //미결등록 텍스트박스 정보 초기화
            textBoxByPassEmpNM2.Text = "";
            textBoxByPassEmpID.Text = "";
            textBoxByPassUserNM2.Text = "";
            textBoxByPassUserID.Text = "";
            this.dateTimePickerByPassUseFrom.Value = DateTime.Now;
            this.dateTimePickerByPassUseTo.Value = DateTime.Now;
            textBoxByPassReason.Text = "";
        }

        private void buttonByPassInsert_Click(object sender, EventArgs e)
        {
            //미결사용자 정보를 등록한다.
            InsertByPass();
        }

        private void buttonByPassUpdate_Click(object sender, EventArgs e)
        {
            //미결사용자 정보를 수정한다.
            UpdateByPass();
            if (dataGridViewByPass.Rows.Count > 0 && dataGridViewByPass.CurrentCell != null)
            {
                SelectDataGridView(dataGridViewByPass, "select_bypass");
            }
            
        }

        private void buttonByPassDelete_Click(object sender, EventArgs e)
        {
            //미결사용자 정보를 삭제한다.
            DeleteByPass();
            if (dataGridViewByPass.Rows.Count > 0 && dataGridViewByPass.CurrentCell != null)
            {
                SelectDataGridView(dataGridViewByPass, "select_bypass");
            }
        }

        #endregion Event










    }
}
