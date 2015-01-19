//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;

using System;
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
    /// 콜센터 학생검색 클래스
    /// </summary>
    /// 
    public partial class FormStudent : Form
    {

         #region Field

        private Common _common;
        private AppMain _appMain;

        #endregion Field

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

        /// <summary>
        /// 기본 생성자
        /// </summary>
        public FormStudent()
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
                //학생검색 콤보박스
                new Common.ComboBoxList(comboBoxCampusType, "캠퍼스구분", true),
                new Common.ComboBoxList(comboBoxCampus, "캠퍼스", true),
                //오답,셀프, 추가학습 콤보박스
                new Common.ComboBoxList(comboBoxCampusTypeMyTest, "캠퍼스구분", true),
                new Common.ComboBoxList(comboBoxCampusMyTest, "캠퍼스", true),
                //맞춤, 만점, 중간학습 콤보박스                
                new Common.ComboBoxList(comboBoxCampusTypeStudyTest, "캠퍼스구분", true),
                new Common.ComboBoxList(comboBoxCampusStudyTest, "캠퍼스", true),
                //학생중복
                new Common.ComboBoxList(comboBoxCampusTypeOverlap, "캠퍼스구분", true),
                new Common.ComboBoxList(comboBoxCampusOverlap, "캠퍼스", true)
            };
            this._common.GetComboList(comboBoxList);
        }

        /// <summary>
        /// 사용자 정의 목록을 조회한다.
        /// </summary>
        /// <history>
        /// 박석제, 2014-09-24, 생성
        /// </history>
        private void SelectDataGridView(DataGridView pDataGridView, string pQueryKind)
        {
            SqlCommand sqlCommand = null;
            SqlResult sqlResult = new SqlResult();

            // 그리드 초기화
            switch (pDataGridView.Name)
            {
                case "dataGridViewStudent":
                    dataGridViewEduStudentClass.Rows.Clear();
                    dataGridViewU2mStudentClass.Rows.Clear();
                    dataGridViewCamMember.Rows.Clear();                    
                    break;
                case "dataGridViewMyTestUser":
                    dataGridViewMyTestRepeat.Rows.Clear();
                    dataGridViewMyTestSet.Rows.Clear();
                    dataGridViewMyTestSetRel.Rows.Clear();
                    
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
                                 row[pDataGridView.Columns[colCount].DataPropertyName].ToString();
                                //pDataGridView[pDataGridView.Columns[colCount].DataPropertyName, pDataGridView.Rows.Count - 1].Value = 
                                //row[pDataGridView.Columns[colCount].DataPropertyName].ToString();
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
            //학생검색
            string businessCD = comboBoxCampusType.SelectedValue.ToString();
            string cpno = comboBoxCampus.SelectedValue.ToString();
            //오답, 셀프 추가학습
            string businessCDMyTest = comboBoxCampusTypeMyTest.SelectedValue.ToString();
            string cpnoMyTest = comboBoxCampusMyTest.SelectedValue.ToString();
            //맞춤, 만점, 중간학습
            string businessCDStudyTest = comboBoxCampusTypeStudyTest.SelectedValue.ToString();
            string cpnoStudyTest = comboBoxCampusStudyTest.SelectedValue.ToString();
            //캠퍼스 학생 반 중복
            string businessCDOverlap = comboBoxCampusTypeOverlap.SelectedValue.ToString();
            string cpnoOverlap = comboBoxCampusOverlap.SelectedValue.ToString();

            

            switch (pQueryKind)
            {
                case "select_u2m_student":
                    // U2M 학생 조회
                    pSqlCommand.CommandText = @"
                       SELECT  C.cpnm
                             , C.business_cd
                             , C.cpno	
                             , C.cpid
                	         , D.login_char
                             , A.userid
                             , A.usernm
                	         , A.member_id
                             , A.login_id
                             , A.login_pwd
                             , A.grade_cd 
					         , A.phone
					         , A.cell
					         , E.pcell
					         , A.use_yn
					         , A.point
                             , D.db_link
                             , D.cp_group_id
                             , A.member_id
                	    FROM tls_member AS A
                   LEFT JOIN tls_cam_member AS B
                	      ON A.userid = B.userid
                   LEFT JOIN tls_campus AS C
                	      ON B.cpno = C.cpno
                  INNER JOIN tls_campus_group AS D
                	      ON C.cp_group_id = D.cp_group_id
				   LEFT JOIN tls_family AS E
					      ON A.userid = E.userid                   
                	   WHERE A.auth_cd = 's' 
                         AND (B.edate = '' OR B.edate IS NULL OR B.sdate <= B.edate)
                          ";

                    if (!string.IsNullOrEmpty(businessCD))
                    {
                        pSqlCommand.CommandText += @"
                         AND C.business_cd = '" + businessCD + "' ";
                    }
                    if (!string.IsNullOrEmpty(cpno))
                    {
                        pSqlCommand.CommandText += @"
                         AND C.cpno = '" + cpno + "' ";
                    }                    
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
                      ORDER BY A.use_yn DESC, C.cpnm, A.usernm";

                    textBoxUserid.Text = "";
                    textBoxLoginID.Text = "";
                    textBoxLoginPW.Text = "";
                    break;
                
                case "select_edu_student_class":
                    //드림+ 학생 반 목록 조회
                    pSqlCommand.CommandText = @"
                       SELECT DISTINCT 
			                  USC.class_id
			                , TC.clno
  			                , TC.clnm
			                , USC.start_date
			                , USC.end_date				
			                , TC.wk_day
			                , (SELECT usernm FROM tls_member WHERE userid = TC.class_tid) AS CLASS_TID
			                , (SELECT name FROM tls_web_code WHERE cdmain = 'grade' AND cdsub = TC.grade_cd) AS GRADE_NM
			                , (SELECT COUNT(userid)FROM tls_class_user WHERE clno = TC.clno AND cpno = TC.cpno AND auth_cd = 's'
				                  AND (end_date = '' OR end_date IS NULL OR CONVERT(CHAR,GETDATE(),112) BETWEEN start_date AND end_date)) AS USER_CNT
		                FROM " + GetCellValue(dataGridViewStudent, dataGridViewStudent.CurrentCell.RowIndex, "db_link") + @".DBO.V_u2m_student_class AS USC WITH(nolock)
                   LEFT JOIN tls_class as TC 
	    	              ON USC.class_id = TC.class_id
    	               WHERE USC.student_id = '" + GetCellValue(dataGridViewStudent, dataGridViewStudent.CurrentCell.RowIndex, "member_id") + @"'
		                 AND TC.cpno = " + GetCellValue(dataGridViewStudent, dataGridViewStudent.CurrentCell.RowIndex, "cpno") + @"
		               ORDER BY USC.end_date ASC, USC.start_date DESC
                    ";
                    break;

                case "select_u2m_student_class":
                    //U2M 학생 반 목록 조회
                    pSqlCommand.CommandText = @"
                       SELECT TC.class_id
               	            , TCU.clno
                            , TC.clnm
                            , TCU.start_date
                            , TCU.end_date
                            , TC.wk_day
                            , TM.usernm
                            , TC.cpno
                            , (SELECT name FROM tls_web_code WHERE cdmain = 'grade' AND cdsub = TC.grade_cd) AS GRADE_NM
               	            , (SELECT COUNT(userid) 
                                 FROM tls_class_user 
                                WHERE clno = TCU.clno AND auth_cd = 's'
								  AND (end_date = '' OR end_date IS NULL OR CONVERT(CHAR,GETDATE(),112) BETWEEN start_date AND end_date)) AS USER_CNT
               	        FROM tls_class_user AS TCU
                   LEFT JOIN tls_class AS TC
              	          ON TCU.clno = TC.clno
                   LEFT JOIN tls_member AS TM
               	          ON TC.class_tid = TM.userid
                       WHERE TCU.userid = " + GetCellValue(dataGridViewStudent, dataGridViewStudent.CurrentCell.RowIndex, "userid") + @"
                         AND (TCU.end_date = '' OR TCU.end_date IS NULL OR TCU.start_date <= TCU.end_date)
               	       ORDER BY TCU.end_date ASC, TCU.start_date DESC
                    ";
                    break;          
       
                case "select_cam_member":
                    //캠퍼스 멤버 조회
                    pSqlCommand.CommandText = @"
                      	SELECT REPLACE(b.cpnm, '캠퍼스', '') AS CPNM
			                 , B.cpno
			                 , A.userid
			                 , A.sdate
			                 , A.edate
			                 , A.udatetime
		                  FROM tls_cam_member AS A
	                 LEFT JOIN tls_campus AS B
			                ON A.cpno = B.cpno
	                 LEFT JOIN tls_member AS C
	                        ON A.userid = C.userid
		                 WHERE A.userid = " + GetCellValue(dataGridViewStudent, dataGridViewStudent.CurrentCell.RowIndex, "userid") + @"
                    ";
                    break;

                case "select_student_study_state":
                    //학생 단말기 수업일별 학습정보를 조회한다.
                    pSqlCommand.CommandText = @"                      	
                        SELECT sddate
                        , sdno
                        , clnm
                        , usernm
                        , login_state
                        , state
                        , bkno
                        , bknm
                        , wk_sort
                        , classa_nm
                        , sdnm
                        , chapter
                        , quiz_count
                        , quiz_type
                        , input_data_1
                        , input_data_2
                        , answer
                        , answer_tf_1
                        , answer_tf_2
                        , result_1
                        , result_2
                        FROM tls_student_study_state_temp 
                        WHERE cpno = " + GetCellValue(dataGridViewStudent, dataGridViewStudent.CurrentCell.RowIndex, "cpno") + @"
                        AND sddate = REPLACE(CONVERT(VARCHAR(10), '" + dateTimePickerStudentStudyState.Value + @"', 112), '-', '.')	            
                        AND clno = " + GetCellValue(dataGridViewU2mStudentClass, dataGridViewU2mStudentClass.CurrentCell.RowIndex, "clno") + @"
                        AND userid = " + GetCellValue(dataGridViewStudent, dataGridViewStudent.CurrentCell.RowIndex, "userid") + @"  
                    ";
                    this.dateTimePickerStudentStudyState.Value = DateTime.Now;

                    break;
                case "select_student_study_state_all":
                    //학생 단말기 전체 학습정보를 조회한다.
                    pSqlCommand.CommandText = @"                      	
                        SELECT sddate
                        , sdno
                        , clnm
                        , usernm
                        , login_state
                        , state
                        , bkno
                        , bknm
                        , wk_sort
                        , classa_nm
                        , sdnm
                        , chapter
                        , quiz_count
                        , quiz_type
                        , input_data_1
                        , input_data_2
                        , answer
                        , answer_tf_1
                        , answer_tf_2
                        , result_1
                        , result_2
                        FROM tls_student_study_state_temp 
                        WHERE cpno = " + GetCellValue(dataGridViewStudent, dataGridViewStudent.CurrentCell.RowIndex, "cpno") + @"                        
                        AND clno = " + GetCellValue(dataGridViewU2mStudentClass, dataGridViewU2mStudentClass.CurrentCell.RowIndex, "clno") + @"
                        AND userid = " + GetCellValue(dataGridViewStudent, dataGridViewStudent.CurrentCell.RowIndex, "userid") + @"  
                    ";
                    this.dateTimePickerStudentStudyState.Value = DateTime.Now;

                    break;

                case "select_mytest_user":
                    //오답, 셀프, 추가학습 배정정보를 조회한다.
                    pSqlCommand.CommandText = @"                      	
                        SELECT C.cpnm
                             , D.clnm
                             , B.usernm
                             , A.cpno
                             , A.clno
                             , A.userid
                             , A.testsetcode
                             , A.test_cd
                             , A.title
                             , A.quiz_cd
                             , A.quiz_cnt
                             , A.end_yn
                             , A.rid
                             , A.rdatetime
                             , A.uid
                             , A.udatetime
                             , B.login_id 
                             , B.login_pwd
                          FROM tls_mytest_user AS A
                     LEFT JOIN tls_member AS B
                               ON A.userid = B.userid
                     LEFT JOIN tls_campus AS C
                               ON A.cpno = C.cpno
                     LEFT JOIN tls_class AS D
                               ON A.clno = D.clno
                         WHERE 1=1    ";

                    if (!string.IsNullOrEmpty(businessCDMyTest))
                    {
                        pSqlCommand.CommandText += @"
                         AND C.business_cd = '" + businessCDMyTest + "' ";
                    }
                    if (!string.IsNullOrEmpty(cpnoMyTest))
                    {
                        pSqlCommand.CommandText += @"
                         AND C.cpno = '" + cpnoMyTest + "' ";
                    }
                    if (!string.IsNullOrEmpty(textBoxCampusMyTest.Text))
                    {
                        pSqlCommand.CommandText += @"
                         AND C.cpnm LIKE '%" + textBoxCampusMyTest.Text + "%' ";
                    }
                    if (!string.IsNullOrEmpty(textBoxUserNmMyTest.Text))
                    {
                        pSqlCommand.CommandText += @"
                         AND (B.usernm LIKE '%" + textBoxUserNmMyTest.Text + "%' ";
                    }
                    if (!string.IsNullOrEmpty(textBoxUserNmMyTest.Text))
                    {
                        pSqlCommand.CommandText += @"
                         OR B.login_id = '" + textBoxUserNmMyTest.Text + "' ";
                    }
                    if (!string.IsNullOrEmpty(textBoxUserNmMyTest.Text))
                    {
                        pSqlCommand.CommandText += @"
                         OR B.userid like '" + textBoxUserNmMyTest.Text + "') ";
                    }
                    if (!string.IsNullOrEmpty(textBoxMyTestTitle.Text))
                    {
                        pSqlCommand.CommandText += @"
                         AND A.title LIKE '%" + textBoxMyTestTitle.Text + "%' ";
                    }
                    if (!string.IsNullOrEmpty(toolStripTextBoxTestSetCode.Text))
                    {
                        pSqlCommand.CommandText += @"
                         AND A.testsetcode = '" + toolStripTextBoxTestSetCode.Text + "' ";
                    }
                    
                    pSqlCommand.CommandText += @"
                       ORDER BY A.rdatetime DESC ";

                    textBoxMyTestTitle.Text = "";                    
                    break;


                case "select_mytest_repeat":
                    //오답,셀프,추가학습 학습 정보를 조회한다.
                    pSqlCommand.CommandText = @"                      	
                        SELECT CASE study_type WHEN 'X' THEN '오답클리닉'
						                       WHEN 'S' THEN '셀프테스트'
						                       WHEN 'A' THEN '추가학습'
		                        END AS STUDY_TYPE
                             , myno
                             , cdate
                             , cpno
                             , userid
                             , school_cd
                             , grade_cd
                             , session_cd
                             , repeatno
                             , title
                             , testsetcode
                             , xtestsetcode
                             , quiz_cnt
                             , end_yn
                             , sdate
                             , edate
                             , rdatetime
                             , udatetime
                          FROM tls_mytest_repeat
                         WHERE cpno = " + GetCellValue(dataGridViewMyTestUser, dataGridViewMyTestUser.CurrentCell.RowIndex, "cpno") + @"                        
                           AND userid = " + GetCellValue(dataGridViewMyTestUser, dataGridViewMyTestUser.CurrentCell.RowIndex, "userid") + @"
                           AND testsetcode = '" + GetCellValue(dataGridViewMyTestUser, dataGridViewMyTestUser.CurrentCell.RowIndex, "testsetcode") + @"'
                         ORDER BY repeatno ";
                    break;

                case "select_mytest_testset":
                    //오답,셀프,추가학습 시험지 정보를 조회한다.
                    pSqlCommand.CommandText = @"                      	
                        SELECT CASE A.study_type WHEN 'X' THEN '오답클리닉'
						                       WHEN 'S' THEN '셀프테스트'
						                       WHEN 'A' THEN '추가학습'
		                        END AS STUDY_TYPE
                             , A.testsetcode
                             , A.test_cd
                             , A.cpno
                             , (select usernm from tls_member where userid = A.userid) AS USERNM
                             , A.userid
                             , A.school_cd
                             , A.grade_cd
                             , A.session_cd
                             , A.title
                             , A.quiz_cd
                             , A.quiz_cnt
                             , A.hard1cnt
                             , A.hard2cnt
                             , A.hard3cnt
                             , A.hard4cnt
                             , A.hard5cnt
                             , A.repeatno_cnt
                             , A.end_yn
                             , A.rid
                             , A.rdatetime
                          FROM tls_mytest_testset AS A
                         WHERE A.cpno = " + GetCellValue(dataGridViewMyTestUser, dataGridViewMyTestUser.CurrentCell.RowIndex, "cpno") + @"                        
                           AND A.userid = " + GetCellValue(dataGridViewMyTestUser, dataGridViewMyTestUser.CurrentCell.RowIndex, "userid") + @"
                           AND A.testsetcode = '" + GetCellValue(dataGridViewMyTestUser, dataGridViewMyTestUser.CurrentCell.RowIndex, "testsetcode") + @"'
                          ";
                    break;

                case "select_mytest_testsetcode":
                    //오답,셀프,추가학습 시험지 정보를 testsetcode로 조회한다.
                    pSqlCommand.CommandText = @"                      	
                        SELECT CASE A.study_type WHEN 'X' THEN '오답클리닉'
						                         WHEN 'S' THEN '셀프테스트'
						                         WHEN 'A' THEN '추가학습'
		                        END AS STUDY_TYPE
                             , A.testsetcode
                             , A.test_cd
                             , A.cpno
                             , (select usernm from tls_member where userid = A.userid) AS USERNM
                             , A.userid
                             , A.school_cd
                             , A.grade_cd
                             , A.session_cd
                             , A.title
                             , A.quiz_cd
                             , A.quiz_cnt
                             , A.hard1cnt
                             , A.hard2cnt
                             , A.hard3cnt
                             , A.hard4cnt
                             , A.hard5cnt
                             , A.repeatno_cnt
                             , A.end_yn
                             , A.rid
                             , A.rdatetime
                          FROM tls_mytest_testset AS A
                         WHERE A.testsetcode = '" + toolStripTextBoxTestSetCode.Text + @"'
                          ";
                    break;

                case "select_mytest_testset_rel":
                    //오답,셀프,추가학습 문항정보를 조회한다.
                    pSqlCommand.CommandText = @"                      	
                        SELECT CASE study_type WHEN 'X' THEN '오답클리닉'
						                         WHEN 'S' THEN '셀프테스트'
						                         WHEN 'A' THEN '추가학습'
		                        END AS STUDY_TYPE
                             , testsetcode
                             , quizcode
                             , orderno
                             , quizno
                             , assignpoints
                             , rdatetime
                          FROM tls_mytest_testset_rel
                         WHERE testsetcode = '" + GetCellValue(dataGridViewMyTestUser, dataGridViewMyTestUser.CurrentCell.RowIndex, "testsetcode") + @"'
                         ORDER BY orderno ";
                    break;

                case "select_study_testset":
                    //맞춤, 만점, 중간학습의 시험지정보를 조회한다.
                    pSqlCommand.CommandText = @"    
		                SELECT (SELECT name FROM tls_web_code 
				                 WHERE cdsub = A.study_type AND cdmain = 'STUDY_TYPE') AS STUDY_TYPE
			                 , B.usernm
			                 , A.testsetcode
		                     , A.yyyy
			                 , A.term_cd
			                 , C.clnm
			                 , D.cpnm
			                 , A.cpno
		 	                 , A.clno
			                 , A.userid
			                 , A.grade_cd
			                 , A.session_cd
			                 , A.bkno
			                 , A.course_cd
			                 , A.sdno
			                 , A.scno
			                 , A.csno
			                 , A.c_apply_date
			                 , A.testkind3
			                 , A.testkind4
		 	                 , A.quiz_cnt
			                 , A.sort
			                 , A.end_yn
			                 , A.rdatetime
		                  FROM tls_study_testset AS A
	                 LEFT JOIN tls_member AS B
	                        ON A.userid = B.userid
                     LEFT JOIN tls_class AS C
			                ON A.clno = C.clno
	                 LEFT JOIN tls_campus AS D
	  	                    ON A.cpno = D.cpno	 
		                 WHERE 1=1 ";                 

                    if (!string.IsNullOrEmpty(businessCDStudyTest))
                    {
                        pSqlCommand.CommandText += @"
                         AND D.business_cd = '" + businessCDStudyTest + "' ";
                    }
                    if (!string.IsNullOrEmpty(cpnoStudyTest))
                    {
                        pSqlCommand.CommandText += @"
                         AND D.cpno = '" + cpnoStudyTest + "' ";
                    }
                    if (!string.IsNullOrEmpty(textBoxCampusStudyTest.Text))
                    {
                        pSqlCommand.CommandText += @"
                         AND D.cpnm LIKE '%" + textBoxCampusStudyTest.Text + "%' ";
                    }
                    if (!string.IsNullOrEmpty(textBoxUserNmStudyTest.Text))
                    {
                        pSqlCommand.CommandText += @"
                         AND (B.usernm LIKE '%" + textBoxUserNmStudyTest.Text + "%' ";
                    }
                    if (!string.IsNullOrEmpty(textBoxUserNmStudyTest.Text))
                    {
                        pSqlCommand.CommandText += @"
                         OR A.userid like '" + textBoxUserNmStudyTest.Text + "') ";
                    }                    
                    pSqlCommand.CommandText += @"
                       ORDER BY A.rdatetime DESC ";
                    
                    break;

                case "select_study_testset_rel":
                    //맞춤, 만점, 중간학습의 시험지정보를 조회한다.
                    pSqlCommand.CommandText = @"
                        SELECT study_type
			                 , testsetcode
			                 , quizcode
			                 , orderno
			                 , quizno
			                 , assignpoints
			                 , rdatetime
		                  FROM tls_study_testset_rel
		                 WHERE testsetcode = '" + GetCellValue(dataGridViewStudyTestSet, dataGridViewStudyTestSet.CurrentCell.RowIndex, "testsetcode") + @"'                
                      ORDER BY orderno ";

                    break;

                case "select_study_test_repeat":
                    //맞춤, 만점, 중간학습의 학습정보를 조회한다.
                    pSqlCommand.CommandText = @"
                        SELECT study_type
			                 , sreno
			                 , yyyy
			                 , term_cd
			                 , cpno
			                 , clno
			                 , userid
			                 , grade_cd
			                 , session_cd
			                 , bkno
			                 , course_cd
			                 , sdno
			                 , scno
			                 , csno
			                 , repeatno
			                 , testsetcode
			                 , xtestsetcode
			                 , quiz_cnt
			                 , end_yn
			                 , sdate
			                 , edate
			                 , rdatetime
			                 , udatetime
		                  FROM tls_study_repeat
		                 WHERE userid = '" + GetCellValue(dataGridViewStudyTestSet, dataGridViewStudyTestSet.CurrentCell.RowIndex, "userid") + @"'                
		                   AND testsetcode = '" + GetCellValue(dataGridViewStudyTestSet, dataGridViewStudyTestSet.CurrentCell.RowIndex, "testsetcode") + @"'                
                      ORDER BY repeatno ";

                    break;

                case "select_student_overlap":
                    //캠퍼스별 수업반 둘 이상인 학생을 조회한다.(오늘일자 수업 반 기준)
                    //130 u2m본사, 139 유투엠FC 테스트, 149 (테)러닝센터, 167 fc캠퍼스, 219 cp테스트 캠퍼스 제외
                    //학년이 없는 반은 조회하지 않는다.
                    pSqlCommand.CommandText = @"                      	
                        SELECT C.cpnm
                             , B.usernm
                             , B.login_id
                             , B.login_pwd
                             , A.userid
                             , b.member_id
                             , COUNT(*) AS CLNO_CNT
                             , C.cp_group_id
                             , C.cpid
                             , C.cpno
                             , D.db_link
                          FROM tls_class_user AS A
                     LEFT JOIN tls_member AS B
                            ON A.userid = B.userid
                     LEFT JOIN tls_campus AS C
                            ON A.cpno = C.cpno
                     LEFT JOIN tls_campus_group AS D
                            ON C.cp_group_id = D.cp_group_id
                     LEFT JOIN tls_class AS E
                            ON A.clno = E.clno
                     LEFT JOIN tls_web_code AS F
							ON E.grade_cd = F.cdsub
                         WHERE C.cpno NOT IN (130, 139, 149, 167, 219)
                           AND A.auth_cd = 'S' 
                           AND (A.end_date IS NULL OR A.end_date = '' OR CONVERT(VARCHAR(8), GETDATE(), 112) BETWEEN A.start_date and A.end_date)
                           AND F.cdmain = 'grade'                               
                                     
                             ";

                    if (!string.IsNullOrEmpty(businessCDOverlap))
                    {
                        pSqlCommand.CommandText += @"
                         AND C.business_cd = '" + businessCDOverlap + "' ";
                    }
                    if (!string.IsNullOrEmpty(cpnoOverlap))
                    {
                        pSqlCommand.CommandText += @"
                         AND C.cpno = '" + cpnoOverlap + "' ";
                    }
                    if (!string.IsNullOrEmpty(textBoxCampusOverlap.Text))
                    {
                        pSqlCommand.CommandText += @"
                         AND C.cpnm LIKE '%" + textBoxCampusOverlap.Text + "%' ";
                    }                    
                    pSqlCommand.CommandText += @"
                       GROUP BY C.cpnm, B.usernm, B.login_id, B.login_pwd, A.userid, b.member_id, C.cp_group_id, D.db_link, C.cpid, C.cpno
                      HAVING COUNT(*) > 1
                       ORDER BY c.cpnm, B.usernm ";
                    
                    break;


                case "select_edu_student_class_overlap":
                    //반 학생중복 탭 드림+ 학생 반 목록 조회
                    pSqlCommand.CommandText = @"
                       SELECT DISTINCT 
			                  USC.class_id
			                , TC.clno
  			                , TC.clnm
			                , USC.start_date
			                , USC.end_date			                
			                , (SELECT name FROM tls_web_code WHERE cdmain = 'grade' AND cdsub = TC.grade_cd) AS GRADE_NM			                
		                FROM " + GetCellValue(dataGridViewStudentOverlap, dataGridViewStudentOverlap.CurrentCell.RowIndex, "db_link") + @".DBO.V_u2m_student_class AS USC WITH(nolock)
                   LEFT JOIN tls_class as TC 
	    	              ON USC.class_id = TC.class_id
    	               WHERE USC.student_id = '" + GetCellValue(dataGridViewStudentOverlap, dataGridViewStudentOverlap.CurrentCell.RowIndex, "member_id") + @"'
		                 AND TC.cpno = " + GetCellValue(dataGridViewStudentOverlap, dataGridViewStudentOverlap.CurrentCell.RowIndex, "cpno") + @"
		               ORDER BY USC.end_date ASC, USC.start_date DESC
                    ";
                    break;

                case "select_u2m_student_class_overlap":
                    //반 학생중복 탭 U2M 학생 반 목록 조회
                    pSqlCommand.CommandText = @"
                       SELECT TC.class_id
               	            , TCU.clno
                            , TC.clnm
                            , TCU.start_date
                            , TCU.end_date                                                        
                            , (SELECT name FROM tls_web_code WHERE cdmain = 'grade' AND cdsub = TC.grade_cd) AS GRADE_NM               	            
               	        FROM tls_class_user AS TCU
                   LEFT JOIN tls_class AS TC
              	          ON TCU.clno = TC.clno
                   LEFT JOIN tls_member AS TM
               	          ON TC.class_tid = TM.userid
                       WHERE TCU.userid = " + GetCellValue(dataGridViewStudentOverlap, dataGridViewStudentOverlap.CurrentCell.RowIndex, "userid") + @"
                         AND (TCU.end_date = '' OR TCU.end_date IS NULL OR TCU.start_date <= TCU.end_date)
               	       ORDER BY TCU.end_date ASC, TCU.start_date DESC
                    ";
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
        /// 학생의 오답, 셀프, 추가학습 배정정보를 삭제한다
        /// </summary>
        /// <history>        
        /// </history>
        private void DeleteMyTestUser()
        {
            if (dataGridViewMyTestUser.Rows.Count > 0 && dataGridViewMyTestUser.CurrentCell != null)
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
                              FROM tls_mytest_user
                             WHERE cpno = '" + this._common.GetCellValue(dataGridViewMyTestUser, dataGridViewMyTestUser.CurrentCell.RowIndex, "cpno") + @"' 
                               AND clno = '" + GetCellValue(dataGridViewMyTestUser, dataGridViewMyTestUser.CurrentCell.RowIndex, "clno") + @"'                              
                               AND userid = '" + GetCellValue(dataGridViewMyTestUser, dataGridViewMyTestUser.CurrentCell.RowIndex, "userid") + @"'
                               AND testsetcode = '" + GetCellValue(dataGridViewMyTestUser, dataGridViewMyTestUser.CurrentCell.RowIndex, "testsetcode") + @"'
                              
                ";
                Console.WriteLine(sqlCommand.CommandText);

                // 처리할 자료가 있을 경우 쿼리실행
                this._common.ExecuteNonQuery(sqlCommand, ref sqlResult);

                this._common.MessageBox(MessageBoxIcon.Information, "자료를 삭제 하였습니다.");

            }
        }
        /// <summary>
        /// 학생의 오답, 셀프, 추가학습 시험지정보를 삭제한다
        /// </summary>
        /// <history>        
        /// </history>
        private void DeleteMyTestSet()
        {
            if (dataGridViewMyTestSet.Rows.Count > 0 && dataGridViewMyTestSet.CurrentCell != null)
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
                              FROM tls_mytest_testset
                             WHERE cpno = '" + this._common.GetCellValue(dataGridViewMyTestSet, dataGridViewMyTestSet.CurrentCell.RowIndex, "cpno") + @"'                               
                               AND userid = '" + GetCellValue(dataGridViewMyTestSet, dataGridViewMyTestSet.CurrentCell.RowIndex, "userid") + @"'
                               AND testsetcode = '" + GetCellValue(dataGridViewMyTestSet, dataGridViewMyTestSet.CurrentCell.RowIndex, "testsetcode") + @"'
                              
                ";
                Console.WriteLine(sqlCommand.CommandText);

                // 처리할 자료가 있을 경우 쿼리실행
                this._common.ExecuteNonQuery(sqlCommand, ref sqlResult);

                this._common.MessageBox(MessageBoxIcon.Information, "자료를 삭제 하였습니다.");

            }
        }

        /// <summary>
        /// 학생의 오답, 셀프, 추가학습 문항정보를 삭제한다
        /// </summary>
        /// <history>        
        /// </history>
        private void DeleteMyTestSetRel()
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
                for (int rowCount = 0; rowCount <= dataGridViewMyTestSetRel.Rows.Count - 1; rowCount++)
                {
                    if (GetCellValue(dataGridViewMyTestSetRel, rowCount, "check_yn") == "1")
                    {
                        isFound = true;
                        sqlCommand.CommandText += @"
                            DELETE tls_mytest_testset_rel 
                             WHERE TESTSETCODE = '" + this._common.GetCellValue(dataGridViewMyTestSetRel, rowCount, "testsetcode") + @"'		                       		                       
                               AND QUIZCODE = '" + this._common.GetCellValue(dataGridViewMyTestSetRel, rowCount, "quizcode") + @"'		                       		                       
                               AND ORDERNO = '" + this._common.GetCellValue(dataGridViewMyTestSetRel, rowCount, "orderno") + @"'		                       		                       
                        ";
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
                    this._common.MessageBox(MessageBoxIcon.Information, "저장할 자료가 없습니다.");
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
        private void FormStudent_Load(object sender, EventArgs e)
        {
            InitCombo();            
        }

        private void dataGridViewStudent_MouseClick(object sender, MouseEventArgs e)
        {
            //학생검색 탭 u2m학습창 및 마이페이지 로그인
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

        private void dataGridViewMyTestUser_MouseClick(object sender, MouseEventArgs e)
        {
            //학생 오답, 셀프, 추가학습 탭 u2m학습창 및 마이페이지 로그인
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
                if (dataGridViewMyTestUser.Rows.Count > 0 && dataGridViewMyTestUser.CurrentCell != null)
                {
                    toolStripTextBoxTestSetCode.Text = "";
                    //학생의 오답, 셀프, 추가학습 학습정보를 검색한다.
                    SelectDataGridView(dataGridViewMyTestRepeat, "select_mytest_repeat");
                    //학생의 오답,셀프,추가학습 시험지 정보를 조회한다.
                    SelectDataGridView(dataGridViewMyTestSet, "select_mytest_testset");
                    //학생의 오답,셀프,추가학습 문항정보를 조회한다.
                    SelectDataGridView(dataGridViewMyTestSetRel, "select_mytest_testset_rel");
                }   
            }
        }

        private void dataGridViewStudentOverlap_MouseClick(object sender, MouseEventArgs e)
        {
            //반 중복학생 탭 u2m학습창 및 마이페이지 로그인
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
                //반 학생중복 드림+ 및 u2m 반 정보를 조회한다.
                if (dataGridViewStudentOverlap.Rows.Count > 0 && dataGridViewStudentOverlap.CurrentCell != null)
                {
                    SelectDataGridView(dataGridViewEduStudentOverlap, "select_edu_student_class_overlap");
                    SelectDataGridView(dataGridViewU2mStudentOverlap, "select_u2m_student_class_overlap");
                }
            }
        }

        /// <summary>
        ///  드림플러스 학생 정보를 유투엠에 연동한다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonImportStudentInfo_Click(object sender, EventArgs e)
        {
            if (dataGridViewStudent.Rows.Count > 0 && dataGridViewStudent.CurrentCell != null)
            {
                if (this._common.MessageBox(MessageBoxIcon.Question, "배치를 실행하시겠습니까?") == System.Windows.Forms.DialogResult.No) return;

                this.Cursor = Cursors.WaitCursor;

                Common.ParametersForImport paramsForImport = new Common.ParametersForImport();
                paramsForImport.AcadGroupId = GetCellValue(dataGridViewStudent, dataGridViewStudent.CurrentCell.RowIndex, "cp_group_id"); ;
                paramsForImport.AcadId = GetCellValue(dataGridViewStudent, dataGridViewStudent.CurrentCell.RowIndex, "cpid"); ;
                paramsForImport.ClassId = "";
                paramsForImport.StudentId = GetCellValue(dataGridViewStudent, dataGridViewStudent.CurrentCell.RowIndex, "member_id"); ;
                paramsForImport.StartDate = "";
                paramsForImport.EndDate = "";

                this._common.ImportDreamPlusStudentInfoToU2M(ref paramsForImport);

                if (paramsForImport.SuccessYn == "N")
                    this._common.MessageBox(MessageBoxIcon.Error, paramsForImport.ErrorMessage);
                else
                    this._common.MessageBox(MessageBoxIcon.Information, "배치가 완료되었습니다.");

                this.Cursor = Cursors.Default;

                SelectDataGridView(dataGridViewStudent, "select_u2m_student");
            }

            
        }

        /// <summary>
        ///  드림플러스 학생 비번 정보를 유투엠에 연동한다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripButtonStudentLoginPW_Click(object sender, EventArgs e)
        {
            if (dataGridViewStudent.Rows.Count > 0 && dataGridViewStudent.CurrentCell != null)
            {
                if (this._common.MessageBox(MessageBoxIcon.Question, "비번 동기화를 실행하시겠습니까?") == System.Windows.Forms.DialogResult.No) return;

                this.Cursor = Cursors.WaitCursor;

                Common.ParametersForImport paramsForImport = new Common.ParametersForImport();
                paramsForImport.UserId = GetCellValue(dataGridViewStudent, dataGridViewStudent.CurrentCell.RowIndex, "userid"); ;

                this._common.SyncDreamPlusPasswordToU2M(ref paramsForImport);

                if (paramsForImport.SuccessYn == "N")
                    this._common.MessageBox(MessageBoxIcon.Error, paramsForImport.ErrorMessage);
                else
                    this._common.MessageBox(MessageBoxIcon.Information, "비번 동기화가 완료되었습니다.");

                this.Cursor = Cursors.Default;

                SelectDataGridView(dataGridViewStudent, "select_u2m_student");
            }
            
        }        

        /// <summary>
        /// 학생검색 캠퍼스 구분 콤보박스 선택 변경시 발생하는 이벤트
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <history>
        /// 박석제, 2014-09-24, 생성
        /// </history>
        private void comboBoxCampusType_SelectedIndexChanged(object sender, EventArgs e)
        {
             //캠퍼스 콤보박스 데이터 생성
            string campusType = comboBoxCampusType.SelectedValue.ToString().Trim();

            _common.GetComboList(comboBoxCampus, "캠퍼스", true, new string[] { campusType });
            //SelectDataGridView(dataGridViewStudent, "select_Student");
        }
        private void textBoxUserNm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //U2M 학생을 검색한다.                
                SelectDataGridView(dataGridViewStudent, "select_u2m_student");                
                textBoxUserid.Text = "";
                textBoxLoginID.Text = "";
                textBoxLoginPW.Text = "";   
            }
        }      
        private void dataGridViewStudent_Click(object sender, EventArgs e)
        {
            if (dataGridViewStudent.Rows.Count > 0 && dataGridViewStudent.CurrentCell != null)
            {
                //드림+ 학생 반 목록을 조회한다.
                SelectDataGridView(dataGridViewEduStudentClass, "select_edu_student_class");
                //U2M 학생 반 목록을 조회한다.
                SelectDataGridView(dataGridViewU2mStudentClass, "select_u2m_student_class");
                //캠퍼스 멤버 조회
                SelectDataGridView(dataGridViewCamMember, "select_cam_member");
                // userid, login_id, login_pw 텍스트박에서 표시한다.  
                textBoxUserid.Text = GetCellValue(dataGridViewStudent, dataGridViewStudent.CurrentCell.RowIndex, "userid");
                textBoxLoginID.Text = GetCellValue(dataGridViewStudent, dataGridViewStudent.CurrentCell.RowIndex, "login_id");
                textBoxLoginPW.Text = GetCellValue(dataGridViewStudent, dataGridViewStudent.CurrentCell.RowIndex, "login_pwd");                
            }                        
        }

        
        
        private void dataGridViewStudent_DoubleClick(object sender, EventArgs e)
        {
            //더블클릭 과정2 학생 차시 조회 폼 이동
            if (dataGridViewStudent.Rows.Count > 0 && dataGridViewStudent.CurrentCell != null)
            {
                FormClassStudentSchedule classStudentSchedule = new FormClassStudentSchedule();
                classStudentSchedule.ClassEmployeeCPNO = this._common.GetCellValue(dataGridViewStudent, dataGridViewStudent.CurrentCell.RowIndex, "cpno");
                classStudentSchedule.ClassStudentUID = this._common.GetCellValue(dataGridViewStudent, dataGridViewStudent.CurrentCell.RowIndex, "userid");
                classStudentSchedule.StudyType = "S";
                classStudentSchedule.Show();
            }
        }

        private void dataGridViewU2mStudentClass_DoubleClick(object sender, EventArgs e)
        {
            //더블클릭 과정1 반 차시 조회 폼 이동
            if (dataGridViewU2mStudentClass.Rows.Count > 0 && dataGridViewU2mStudentClass.CurrentCell != null)
            {
                if (dataGridViewU2mStudentClass.Columns[dataGridViewU2mStudentClass.CurrentCell.ColumnIndex].DataPropertyName != "check_yn")
                {
                    FormClassStudentSchedule classStudentSchedule = new FormClassStudentSchedule();
                    classStudentSchedule.StudyType = "C";
                    classStudentSchedule.ClassEmployeeCPNO = this._common.GetCellValue(dataGridViewU2mStudentClass, dataGridViewU2mStudentClass.CurrentCell.RowIndex, "cpno");
                    classStudentSchedule.ClassEmployeeCLNO = this._common.GetCellValue(dataGridViewU2mStudentClass, dataGridViewU2mStudentClass.CurrentCell.RowIndex, "clno");
                    classStudentSchedule.Show();
                }
            }
        }

        private void buttonStudentStudyState_Click(object sender, EventArgs e)
        {
            //학생 단말기 수업일별 학습정보를 조회한다.
            if (dataGridViewU2mStudentClass.Rows.Count > 0 && dataGridViewU2mStudentClass.CurrentCell != null)
            {
                SelectDataGridView(dataGridViewStudentStudyState, "select_student_study_state");
            }
        }
        private void buttonStudentStudyStateAll_Click(object sender, EventArgs e)
        {
            //학생 단말기 전체 학습정보를 조회한다.
            if (dataGridViewU2mStudentClass.Rows.Count > 0 && dataGridViewU2mStudentClass.CurrentCell != null)
            {
                SelectDataGridView(dataGridViewStudentStudyState, "select_student_study_state_all");
            }
        }
        private void dataGridViewStudentStudyState_Click(object sender, EventArgs e)
        {
            //단말기 학습정보 클릭 시 dateTimePicker를 해당 날짜로 바꾼다.
            this.dateTimePickerStudentStudyState.Value = DateTime.Parse(this._common.GetCellValue(dataGridViewStudentStudyState, dataGridViewStudentStudyState.CurrentCell.RowIndex, "sddate"));
        }
        


        private void comboBoxCampusTypeMyTest_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //오답, 셀프, 추가학습 캠퍼스 콤보박스 데이터 생성
            string campusType = comboBoxCampusTypeMyTest.SelectedValue.ToString().Trim();

            _common.GetComboList(comboBoxCampusMyTest, "캠퍼스", true, new string[] { campusType });
        }

        private void textBoxCampusMyTest_KeyDown(object sender, KeyEventArgs e)
        {
            string businessCDMyTest = comboBoxCampusTypeMyTest.SelectedValue.ToString();

            if (!string.IsNullOrEmpty(businessCDMyTest))
            {
                if (e.KeyCode == Keys.Enter)
                {
                    toolStripTextBoxTestSetCode.Text = "";
                    //캠퍼스 오답, 셀프, 추가학습 배정정보를 검색한다.                 
                    SelectDataGridView(dataGridViewMyTestUser, "select_mytest_user");
                }
            }            
        }

        private void textBoxUserNmMyTest_KeyDown(object sender, KeyEventArgs e)
        {
            string businessCDMyTest = comboBoxCampusTypeMyTest.SelectedValue.ToString();

            if (!string.IsNullOrEmpty(businessCDMyTest))
            {
                if (e.KeyCode == Keys.Enter)
                {
                    toolStripTextBoxTestSetCode.Text = "";
                    //학생의 오답, 셀프, 추가학습 배정정보를 검색한다.                 
                    SelectDataGridView(dataGridViewMyTestUser, "select_mytest_user");
                }
            }
            

            
        }

        private void textBoxMyTestTitle_KeyDown(object sender, KeyEventArgs e)
        {
            string businessCDMyTest = comboBoxCampusTypeMyTest.SelectedValue.ToString();

            if (!string.IsNullOrEmpty(businessCDMyTest))
            {
                if (e.KeyCode == Keys.Enter)
                {
                    toolStripTextBoxTestSetCode.Text = "";
                    //학생의 오답, 셀프, 추가학습 배정정보의 title을 검색한다.                
                    SelectDataGridView(dataGridViewMyTestUser, "select_mytest_user");
                }
            }            
            
        }

        private void buttonMyTestTestSetCode_Click(object sender, EventArgs e)
        {
            if (dataGridViewMyTestUser.Rows.Count > 0 && dataGridViewMyTestUser.CurrentCell != null)
            {
                //오답, 셀프, 추가학습 배정정보의 testsetcode를 텍스트박스에 입력한다.
                toolStripTextBoxTestSetCode.Text = GetCellValue(dataGridViewMyTestUser, dataGridViewMyTestUser.CurrentCell.RowIndex, "testsetcode");
                if (!string.IsNullOrEmpty(toolStripTextBoxTestSetCode.Text))
                {
                    //오답, 셀프, 추가학습 배정정보를 testsetcode로 조회한다.                
                    textBoxUserNmMyTest.Text = "";
                    SelectDataGridView(dataGridViewMyTestUser, "select_mytest_user");
                    SelectDataGridView(dataGridViewMyTestSet, "select_mytest_testsetcode");
                    SelectDataGridView(dataGridViewMyTestSetRel, "select_mytest_testset_rel");
                }
            }
            
        }
        private void buttonDeleteMyTest_Click(object sender, EventArgs e)
        {
            if (dataGridViewMyTestUser.Rows.Count > 0 && dataGridViewMyTestUser.CurrentCell != null)
            {
                //학생의 오답, 셀프, 추가학습 배정정보를 삭제한다.
                DeleteMyTestUser();
            }
            
        }
        private void buttonDeleteMyTestRepeat_Click(object sender, EventArgs e)
        {
            //학생의 오답, 셀프, 추가학습 학습정보(학습이력)를 삭제한다.
        }
        private void buttonDeleteMyTestSetRel_Click(object sender, EventArgs e)
        {
            if (dataGridViewMyTestSetRel.Rows.Count > 0 && dataGridViewMyTestSetRel.CurrentCell != null)
            {
                //오답, 셀프, 추가학습 문항정보를 삭제한다.
                DeleteMyTestSetRel();
            }            
        }

        private void buttonDeleteMyTestSet_Click(object sender, EventArgs e)
        {
            if (dataGridViewMyTestSet.Rows.Count > 0 && dataGridViewMyTestSet.CurrentCell != null)
            {
                //오답, 셀프, 추가학습 시험지정보를 삭제한다.
                DeleteMyTestSet();
            }
            
        }


        private void comboBoxCampusTypeOverlap_SelectedIndexChanged(object sender, EventArgs e)
        {
            //반 중복학생 캠퍼스 콤보박스 데이터 생성
            string campusType = comboBoxCampusTypeOverlap.SelectedValue.ToString().Trim();

            _common.GetComboList(comboBoxCampusOverlap, "캠퍼스", true, new string[] { campusType });
        }

        private void buttonStudentOverlap_Click(object sender, EventArgs e)
        {
            //반 학생 중복 검색 
            SelectDataGridView(dataGridViewStudentOverlap, "select_student_overlap");

        }

        private void textBoxCampusOverlap_KeyDown(object sender, KeyEventArgs e)
        {
            //캠퍼스별 반 중복 학생을 조회한다.
            if (e.KeyCode == Keys.Enter)
            {
                SelectDataGridView(dataGridViewStudentOverlap, "select_student_overlap");
            }
        }

        private void dataGridViewStudentOverlap_KeyDown(object sender, KeyEventArgs e)
        {
            //반 중복학생 선택 Ctrl + 1, 2, 3 체크박스 선택
            if (e.Control && (e.KeyCode == Keys.D1 || e.KeyCode == Keys.D2 || e.KeyCode == Keys.D3))
                _common.GridCheck((DataGridView)sender, e);
        }       

        private void buttonStudentOverlapImport_Click(object sender, EventArgs e)
        {
            //캠퍼스별 반 중복학생을 제거한다.
            if (dataGridViewStudentOverlap.Rows.Count > 0 && dataGridViewStudentOverlap.CurrentCell != null)
            {
                Boolean isFound = false; // 처리할 자료가 있는지 체크할 변수
                DialogResult result = this._common.MessageBox(MessageBoxIcon.Question, "배치 진행 하시겠습니까?");
                if (result == DialogResult.No)
                {
                    return;
                }

                SqlCommand sqlCommand = new SqlCommand();
                SqlResult sqlResult = new SqlResult();

                this.Cursor = Cursors.WaitCursor;
                for (int rowCount = 0; rowCount <= dataGridViewStudentOverlap.Rows.Count - 1; rowCount++)
                {
                    if (GetCellValue(dataGridViewStudentOverlap, rowCount, "check_yn") == "1")
                    {
                        isFound = true;
                        //if (this._common.MessageBox(MessageBoxIcon.Question, "배치를 실행하시겠습니까?") == System.Windows.Forms.DialogResult.No) return;

                        this.Cursor = Cursors.WaitCursor;

                        Common.ParametersForImport paramsForImport = new Common.ParametersForImport();
                        paramsForImport.AcadGroupId = GetCellValue(dataGridViewStudentOverlap, rowCount, "cp_group_id"); ;
                        paramsForImport.AcadId = GetCellValue(dataGridViewStudentOverlap, rowCount, "cpid"); ;
                        paramsForImport.ClassId = "";
                        paramsForImport.StudentId = GetCellValue(dataGridViewStudentOverlap, rowCount, "member_id"); ;
                        paramsForImport.StartDate = "";
                        paramsForImport.EndDate = "";

                        this._common.ImportDreamPlusStudentInfoToU2M(ref paramsForImport);

                        if (paramsForImport.SuccessYn == "N")
                            this._common.MessageBox(MessageBoxIcon.Error, paramsForImport.ErrorMessage);
                        //else
                        //    this._common.MessageBox(MessageBoxIcon.Information, "배치가 완료되었습니다.");

                        this.Cursor = Cursors.Default;
                    }
                }
                if (isFound == true)
                {
                    this._common.MessageBox(MessageBoxIcon.Information, "배치가 완료되었습니다.");
                }
                //{
                //    // 처리할 자료가 있을 경우 쿼리실행
                //    this._common.ExecuteNonQuery(sqlCommand, ref sqlResult);

                //    if (sqlResult.Success == true)
                //    {
                //        // 작업 성공시
                //        if (sqlResult.AffectedRecords > 0)
                //            this._common.MessageBox(MessageBoxIcon.Information, "배치가 완료 되었습니다.." + Environment.NewLine +
                //                string.Format("(배치된 자료건 수 총 : {0}건)", sqlResult.AffectedRecords));
                //        else
                //            this._common.MessageBox(MessageBoxIcon.Information, "배치된 자료가 없습니다.");
                //    }
                //    else
                //        // 작업 실패시
                //        MessageBox.Show(sqlResult.ErrorMsg);
                //}
                //else
                //    // 처리할 자료가 없을 경우
                //    this._common.MessageBox(MessageBoxIcon.Information, "배치할 자료가 없습니다.");

            }

        }

        #endregion Event

        private void comboBoxCampusTypeStudyTest_SelectionChangeCommitted(object sender, EventArgs e)
        {
            //맞춤, 만점, 중간 콤보박스 데이터 생성
            string campusType = comboBoxCampusTypeStudyTest.SelectedValue.ToString().Trim();

            _common.GetComboList(comboBoxCampusStudyTest, "캠퍼스", true, new string[] { campusType });
        }

        private void textBoxCampusStudyTest_KeyDown(object sender, KeyEventArgs e)
        {
            //맞춤, 만점, 중간학습 캠퍼스 시험지 정보를 조회한다.
            string businessCDStudyTest = comboBoxCampusTypeStudyTest.SelectedValue.ToString();

            if (!string.IsNullOrEmpty(businessCDStudyTest))
            {
                if (e.KeyCode == Keys.Enter)
                {                    
                    SelectDataGridView(dataGridViewStudyTestSet, "select_study_testset");
                }
            }
        }

        private void textBoxUserNmStudyTest_KeyDown(object sender, KeyEventArgs e)
        {
            //맞춤, 만점, 중간학습 학생의 시험지 정보를 조회한다.
            string businessCDStudyTest = comboBoxCampusTypeStudyTest.SelectedValue.ToString();

            if (!string.IsNullOrEmpty(businessCDStudyTest))
            {
                if (e.KeyCode == Keys.Enter)
                {
                    SelectDataGridView(dataGridViewStudyTestSet, "select_study_testset");
                }
            }  
        }

        private void dataGridViewStudyTestSet_Click(object sender, EventArgs e)
        {
            //맞춤, 만점, 중간학습 학생시험지의 문항 정보를 조회한다.
            if (dataGridViewStudyTestSet.Rows.Count > 0 && dataGridViewStudyTestSet.CurrentCell != null)
            {
                SelectDataGridView(dataGridViewStudyTestSetRel, "select_study_testset_rel");
                SelectDataGridView(dataGridViewStudyTestRepeat, "select_study_test_repeat");
            }
        }

        

        

        

        

        





























    }
}
