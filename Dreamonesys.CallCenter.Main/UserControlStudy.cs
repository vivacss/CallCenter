﻿//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Drawing;
//using System.Data;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows.Forms;


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
    public partial class UserControlStudy : UserControl
    {
        #region Field

        private Common _common;
        private AppMain _appMain;

        #endregion

        #region Property

        public string StudyType { get; set; }
        public string ClassEmployeeCPNO { get; set; }
        public string ClassEmployeeCLNO { get; set; }
        public string ClassStudentCPNO { get; set; }
        public string ClassStudentUID { get; set; }
        public string ClassEmployeeUID { get; set; }
        public string ClassSchoolCDStudy { get; set; }
        

        #endregion

        #region Constructor

        public UserControlStudy()
        {
            InitializeComponent();

            // 공통 모듈 클래스 인스턴스 생성
            _common = new Common();
            // 프로그램 정보 클래스 인스턴스 생성
            _appMain = new AppMain();
            // 공용 모듈에서 프로그램 정보를 참조할 수 있도록 함
            _common._appMain = _appMain;
            // 프로그램 정보에서 메인 폼을 참조할 수 있도록 함
            //_appMain.MainForm = this;
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
                //new Common.ComboBoxList(comboBoxCampusType, "캠퍼스구분", true),
                //new Common.ComboBoxList(comboBoxCampus, "캠퍼스", true),            
                                
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
                case "dataGridViewClassStudy":
                    dataGridViewClassSchedule.Rows.Clear();
                    break;
                case "dataGridViewStudentStudy":
                    dataGridViewStudentSchedule.Rows.Clear();
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
                            pDataGridView[colCount, pDataGridView.Rows.Count - 1].Value =
                                //dataGridViewCampus.Rows[dataGridViewCampus.Rows.Count - 1].Cells[colCount].Value = 
                                row[pDataGridView.Columns[colCount].DataPropertyName].ToString();
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
            //string businessCD = comboBoxCampusType.SelectedValue.ToString();
            //string cpno = comboBoxCampus.SelectedValue.ToString();

            switch (pQueryKind)
            {

                case "select_class_study":

                    //반 차시 정보 조회(과정1) 
                    pSqlCommand.CommandText = @"                       
		                SELECT (SELECT usernm FROM tls_member WHERE userid = CS.tid) AS TID
		                     , (SELECT cpnm FROM tls_campus WHERE cpno = CS.cpno) AS CPNM
                             , CS.term_cd
			                 , TC.clnm
			                 , STUFF(STUFF(CS.sdate, 5, 0, '-'), 8, 0, '-') AS SDATE
			                 , STUFF(STUFF(CS.edate, 5, 0, '-'), 8, 0, '-') AS EDATE
			                 , DBO.F_U_WEEK_HAN(CS.week_day) AS WEEK_DAY
			                 , (TS.sdnm + view_sdnm) AS SDNM
                             , CS.sdno
                             , TS.bkno
			                 , CS.j_use_yn
			                 , CS.j_count
			                 , CS.j_hitpoint
			                 , CS.j_quiz_cnt
			                 , CS.correct_yn
			                 , CS.c_use_yn
			                 , CS.c_common_cnt
			                 , CS.c_each_cnt
			                 , CS.l_quiz_cnt
			                 , CS.concept_yn
			                 , CS.quiz_yn
			                 , CS.menu_yn
			                 , (SELECT usernm FROM tls_member WHERE userid = CS.rid) AS RID
			                 , CS.RDATETIME
			                 , (SELECT usernm FROM tls_member WHERE userid = CS.uid) AS UID
			                 , CS.UDATETIME
                             , CS.yyyy
                             , CS.cpno
                             , CS.clno
		                  FROM tls_class_study AS CS
                     LEFT JOIN tls_class AS TC
	                        ON CS.cpno = TC.cpno and CS.clno = TC.clno
	                 LEFT JOIN tls_study AS TS
	                        ON CS.sdno = TS.sdno
                     LEFT JOIN tls_campus AS CA
                            ON TC.cpno = CA.cpno
		                 WHERE CONVERT(CHAR,GETDATE(), 112) BETWEEN CS.sdate AND CS.edate		            
                                            ";
                    if (!string.IsNullOrEmpty(ClassEmployeeCPNO))
                    {
                        pSqlCommand.CommandText += @"
                         AND CS.cpno = '" + ClassEmployeeCPNO + "' ";
                    }
                    if (!string.IsNullOrEmpty(ClassEmployeeCLNO))
                    {
                        pSqlCommand.CommandText += @"
                         AND CS.clno = '" + ClassEmployeeCLNO + "' ";
                    }
                    if (!string.IsNullOrEmpty(ClassEmployeeUID))
                    {
                        pSqlCommand.CommandText += @"
                         AND CS.tid = '" + ClassEmployeeUID + "' ";
                    }
                    
                    //                    if (!string.IsNullOrEmpty(businessCD))
                    //                    {
                    //                        pSqlCommand.CommandText += @"
                    //                         AND CA.business_cd = '" + businessCD + "' ";                        
                    //                    }
                    //                    if (!string.IsNullOrEmpty(cpno))
                    //                    {
                    //                        pSqlCommand.CommandText += @"
                    //                         AND CA.cpno = '" + cpno + "' ";                        
                    //                    }
                    pSqlCommand.CommandText += @"
                        ORDER BY TC.clnm, CS.sdate ";
                    break;

                case "select_class_study_all":

                    //반별, 학습별 차시 정보 조회(과정1) 
                    pSqlCommand.CommandText = @"                       
		                SELECT (SELECT usernm FROM tls_member WHERE userid = CS.tid) AS TID
		                     , (SELECT cpnm FROM tls_campus WHERE cpno = CS.cpno) AS CPNM
                             , CS.term_cd
			                 , TC.clnm
			                 , STUFF(STUFF(CS.sdate, 5, 0, '-'), 8, 0, '-') AS SDATE
			                 , STUFF(STUFF(CS.edate, 5, 0, '-'), 8, 0, '-') AS EDATE
			                 , DBO.F_U_WEEK_HAN(CS.week_day) AS WEEK_DAY
			                 , (TS.sdnm + '-' + view_sdnm) AS SDNM
                             , CS.sdno
                             , TS.bkno
			                 , CS.j_use_yn
			                 , CS.j_count
			                 , CS.j_hitpoint
			                 , CS.j_quiz_cnt
			                 , CS.correct_yn
			                 , CS.c_use_yn
			                 , CS.c_common_cnt
			                 , CS.c_each_cnt
			                 , CS.l_quiz_cnt
			                 , CS.concept_yn
			                 , CS.quiz_yn
			                 , CS.menu_yn
			                 , (SELECT usernm FROM tls_member WHERE userid = CS.rid) AS rid
			                 , CS.RDATETIME
			                 , (SELECT usernm FROM tls_member WHERE userid = CS.uid) AS uid
			                 , CS.UDATETIME
                             , CS.yyyy
                             , CS.cpno
                             , CS.clno
                             , TC.school_cd
		                  FROM tls_class_study AS CS
                     LEFT JOIN tls_member AS TM
							ON CS.tid = TM.userid
                     LEFT JOIN tls_class AS TC
	                        ON CS.cpno = TC.cpno and CS.clno = TC.clno
	                 LEFT JOIN tls_study AS TS
	                        ON CS.sdno = TS.sdno
                     LEFT JOIN tls_campus AS CA
                            ON TC.cpno = CA.cpno
		                 WHERE 1=1
                    ";
                    if (!string.IsNullOrEmpty(ClassEmployeeCPNO))
                    {
                        pSqlCommand.CommandText += @"
                         AND CS.cpno = '" + ClassEmployeeCPNO + "' ";
                    }
                    if (!string.IsNullOrEmpty(ClassSchoolCDStudy))
                    {
                        pSqlCommand.CommandText += @"
                         AND  = TC.school_cd '" + ClassSchoolCDStudy + "' ";
                    }

//                    if (!string.IsNullOrEmpty(businessCD))
//                    {
//                        pSqlCommand.CommandText += @"
//                            AND CA.business_cd = '" + businessCD + "' ";
//                    }
//                    if (!string.IsNullOrEmpty(cpno))
//                    {
//                        pSqlCommand.CommandText += @"
//                            AND CA.cpno = '" + cpno + "' ";
//                    }
                    if (!string.IsNullOrEmpty(toolStripTextBoxClassNM.Text))
                    {
                        pSqlCommand.CommandText += @"
                            AND TC.clnm LIKE '%" + toolStripTextBoxClassNM.Text + "%' ";
                    }
                    if (!string.IsNullOrEmpty(toolStripTextBoxStudyNM.Text))
                    {
                        pSqlCommand.CommandText += @"
                            AND TS.sdnm LIKE '%" + toolStripTextBoxStudyNM.Text + "%' ";
                    }
                    if (!string.IsNullOrEmpty(toolStripTextBoxClassTID.Text))
                    {
                        pSqlCommand.CommandText += @"
                            AND TM.usernm LIKE '%" + toolStripTextBoxClassTID.Text + "%' ";
                    }
                    pSqlCommand.CommandText += @"                      
                            AND REPLACE(CONVERT(VARCHAR(10), '" + dateTimePickerClassStudy.Value + @"', 112), '-', '') BETWEEN CS.sdate AND CS.edate		            
                        ORDER BY TC.clnm, CS.sdate
                    ";
                    toolStripTextBoxClassNM.Text = "";
                    toolStripTextBoxStudyNM.Text = "";
                    toolStripTextBoxClassTID.Text = "";
                    toolStripTextBoxClassBookNM.Text = "";
                    toolStripTextBoxClassDataTime.Text = "";
                    break;
                case "select_class_study_datatime_all":

                    //반별, 학습별 차시 정보 조회(과정1) - 수업일 전체조회
                    pSqlCommand.CommandText = @"                       
		                SELECT (SELECT usernm FROM tls_member WHERE userid = CS.tid) AS TID
		                     , (SELECT cpnm FROM tls_campus WHERE cpno = CS.cpno) AS CPNM
                             , CS.term_cd
			                 , TC.clnm
			                 , STUFF(STUFF(CS.sdate, 5, 0, '-'), 8, 0, '-') AS SDATE
			                 , STUFF(STUFF(CS.edate, 5, 0, '-'), 8, 0, '-') AS EDATE
			                 , DBO.F_U_WEEK_HAN(CS.week_day) AS WEEK_DAY
			                 , (TS.sdnm + '-' + view_sdnm) AS SDNM
                             , CS.sdno
                             , TS.bkno
			                 , CS.j_use_yn
			                 , CS.j_count
			                 , CS.j_hitpoint
			                 , CS.j_quiz_cnt
			                 , CS.correct_yn
			                 , CS.c_use_yn
			                 , CS.c_common_cnt
			                 , CS.c_each_cnt
			                 , CS.l_quiz_cnt
			                 , CS.concept_yn
			                 , CS.quiz_yn
			                 , CS.menu_yn
			                 , (SELECT usernm FROM tls_member WHERE userid = CS.rid) AS rid
			                 , CS.RDATETIME
			                 , (SELECT usernm FROM tls_member WHERE userid = CS.uid) AS uid
			                 , CS.UDATETIME
                             , CS.yyyy
                             , CS.cpno
                             , CS.clno
		                  FROM tls_class_study AS CS
                     LEFT JOIN tls_member AS TM
							ON CS.tid = TM.userid
                     LEFT JOIN tls_class AS TC
	                        ON CS.cpno = TC.cpno and CS.clno = TC.clno
	                 LEFT JOIN tls_study AS TS
	                        ON CS.sdno = TS.sdno
                     LEFT JOIN tls_campus AS CA
                            ON TC.cpno = CA.cpno
		                 WHERE 1=1
                    ";
                    if (!string.IsNullOrEmpty(ClassEmployeeCPNO))
                    {
                        pSqlCommand.CommandText += @"
                         AND CS.cpno = '" + ClassEmployeeCPNO + "' ";
                    }
                     if (!string.IsNullOrEmpty(toolStripTextBoxClassNM.Text))
                    {
                        pSqlCommand.CommandText += @"
                            AND TC.clnm LIKE '%" + toolStripTextBoxClassNM.Text + "%' ";
                    }
                    if (!string.IsNullOrEmpty(toolStripTextBoxStudyNM.Text))
                    {
                        pSqlCommand.CommandText += @"
                            AND TS.sdnm LIKE '%" + toolStripTextBoxStudyNM.Text + "%' ";
                    }
                    if (!string.IsNullOrEmpty(toolStripTextBoxClassTID.Text))
                    {
                        pSqlCommand.CommandText += @"
                            AND TM.usernm LIKE '%" + toolStripTextBoxClassTID.Text + "%' ";
                    }                    
                    pSqlCommand.CommandText += @"                                                 		            
                        ORDER BY CA.cpnm, TC.clnm, CS.sdate DESC
                    ";
                    toolStripTextBoxClassNM.Text = "";
                    toolStripTextBoxStudyNM.Text = "";
                    toolStripTextBoxClassTID.Text = "";
                    toolStripTextBoxClassBookNM.Text = "";
                    toolStripTextBoxClassDataTime.Text = "";
                    break;

                case "select_class_schedule":

                    //반 차시 리스트 
                    pSqlCommand.CommandText += @"
                        SELECT STUFF(STUFF(A.cdate, 5, 0, '-'), 8, 0, '-') AS CDATE
			                 , DBO.F_U_WEEK_HAN(A.cweek_cd) AS CWEEK_CD
			                 , CONVERT(VARCHAR(2), F.sort) + '단원' + ' ' +  CONVERT(VARCHAR(2), E.time_cnt) + '차시' AS TIME_CNT
			                 , B.bknm
			                 , D.sdnm + '-' + D.view_sdnm AS SDNM
			                 , (SELECT name1 FROM tbl_quiz_class WHERE classa = F.classa) AS CLASSA
			                 , G.view_unnm
			                 , H.view_dfnm AS DFNM
			                 , CASE G.room_code WHEN 'P' THEN 'SMART Room'
					 	                        WHEN 'I' THEN 'LESSON Room'
						                        WHEN 'S' THEN 'SDL Room'
			                   END AS ROOM_CODE
			                 , CASE G.study_type_1 WHEN 'PC' THEN 'PC-(동+문)'
						                           WHEN 'PM' THEN 'PM-(동+문)'
						                           WHEN 'PQ' THEN 'PQ-(동+문)'
						                           WHEN 'E' THEN '채점'
							                       WHEN 'R' THEN '단말기'
							                       WHEN 'Q' THEN '문제풀이'
							                       WHEN 'M' THEN '동영상'
			                   END AS study_type_1
			                 , CASE G.study_type_2 WHEN 'PC' THEN 'PC-(동+문)'
							                       WHEN 'PM' THEN 'PM-(동+문)'
							                       WHEN 'PQ' THEN 'PQ-(동+문)'
							                       WHEN 'E' THEN '채점'
							                       WHEN 'R' THEN '단말기'
							                       WHEN 'Q' THEN '문제풀이'
							                       WHEN 'M' THEN '동영상'
			                   END AS study_type_2			                         		
		                  FROM tls_class_schedule AS A
                    INNER JOIN tls_book AS B 
	                        ON B.bkno = A.bkno
                    INNER JOIN tls_study AS D 
                            ON D.sdno = A.sdno
                    INNER JOIN tls_schedule AS E 
		                    ON E.sdno = A.sdno AND E.scno = A.scno
                    INNER JOIN TLS_CHAPTER AS F 
		                    ON F.bkno = B.bkno AND F.chno = E.chno
                    INNER JOIN tls_unit AS G 
		                    ON G.bkno = B.bkno AND G.chno = F.chno AND G.unno = E.unno
                    INNER JOIN tls_lvl_def AS H 
		                    ON H.lvno = G.lvno AND H.dfno = G.dfno
	                     WHERE A.yyyy = '" + this._common.GetCellValue(dataGridViewClassStudy, dataGridViewClassStudy.CurrentCell.RowIndex, "yyyy") + @"'
	                       AND A.term_cd = '" + this._common.GetCellValue(dataGridViewClassStudy, dataGridViewClassStudy.CurrentCell.RowIndex, "term_cd") + @"'
		                   AND A.cpno = '" + this._common.GetCellValue(dataGridViewClassStudy, dataGridViewClassStudy.CurrentCell.RowIndex, "cpno") + @"'
		                   AND A.clno = '" + this._common.GetCellValue(dataGridViewClassStudy, dataGridViewClassStudy.CurrentCell.RowIndex, "clno") + @"'
		                   AND A.sdno = '" + this._common.GetCellValue(dataGridViewClassStudy, dataGridViewClassStudy.CurrentCell.RowIndex, "sdno") + @"'
                    ";
                    if (!string.IsNullOrEmpty(toolStripTextBoxClassBookNM.Text))
                    {
                        pSqlCommand.CommandText += @"
                            AND G.view_unnm LIKE '%" + toolStripTextBoxClassBookNM.Text + "%' ";
                    }
                    if (!string.IsNullOrEmpty(toolStripTextBoxClassDataTime.Text))
                    {
                        pSqlCommand.CommandText += @"
                            AND A.cdate LIKE '" + toolStripTextBoxClassDataTime.Text + "%' ";
                    }
                    pSqlCommand.CommandText += @"
	                     ORDER BY A.cdate, G.sort
                    ";
                    toolStripTextBoxClassBookNM.Text = "";
                    toolStripTextBoxClassDataTime.Text = "";
                    break;
                case "select_student_study":

                    //학생 차시 정보 조회(과정2)
                    pSqlCommand.CommandText = @"                       
		                SELECT (SELECT usernm FROM tls_member WHERE userid = MS.tid) AS TID
	    	                 , (SELECT cpnm FROM tls_campus WHERE cpno = MS.cpno) AS CPNM
                             , MS.term_cd
		                     , TC.clnm
                             , (SELECT usernm FROM tls_member where userid = ms.userid) AS USERNM
			                 , STUFF(STUFF(MS.sdate, 5, 0, '-'), 8, 0, '-') AS SDATE 
	                         , STUFF(STUFF(MS.edate, 5, 0, '-'), 8, 0, '-') AS EDATE
	 		                 , DBO.F_U_WEEK_HAN(MS.week_day) AS WEEK_DAY
			                 , (TS.sdnm + view_sdnm) AS SDNM
                             , MS.sdno
                             , TS.bkno
			                 , MS.use_yn
			                 , MS.j_use_yn
			                 , MS.j_count
			                 , MS.j_hitpoint
			                 , MS.j_quiz_cnt
			                 , MS.correct_yn
			                 , MS.m_use_yn
			                 , MS.m_count			 
			                 , MS.m_hitpoint
			                 , MS.m_quiz_cnt
			                 , MS.m_quiz_type
			                 , MS.l_quiz_cnt
			                 , MS.concept_yn
			                 , MS.quiz_yn
			                 , MS.menu_yn
			                 , (SELECT usernm FROM tls_member WHERE userid = MS.rid) AS RID
			                 , MS.RDATETIME
			                 , (SELECT usernm FROM tls_member WHERE userid = MS.uid) AS UID
			                 , MS.UDATETIME
                             , MS.yyyy
                             , MS.cpno
                             , MS.userid
                             , (SELECT login_id FROM tls_member where userid = ms.userid) AS LOGIN_ID
                             , (SELECT login_pwd FROM tls_member where userid = ms.userid) AS LOGIN_PWD
	                     FROM tls_member_study AS MS
                    LEFT JOIN tls_class AS TC
	                       ON MS.cpno = TC.cpno and MS.clno = TC.clno
	                LEFT JOIN tls_study AS TS
	                       ON MS.sdno = TS.sdno
		                WHERE MS.cpno = " + ClassEmployeeCPNO + @"
                          AND MS.userid = " + ClassStudentUID + @"
		                  AND CONVERT(CHAR, GETDATE(), 112) BETWEEN MS.sdate AND MS.edate
                        ORDER BY MS.sdate
		            ";
                    break;

                case "select_student_study_all":

                    //반별 학생, 학습명 차시 정보 조회(과정2)
                    pSqlCommand.CommandText = @"                       
		                SELECT (SELECT usernm FROM tls_member WHERE userid = MS.tid) AS TID
	    	                 , (SELECT cpnm FROM tls_campus WHERE cpno = MS.cpno) AS CPNM
                             , MS.term_cd
		                     , TC.clnm
                             , TM.usernm
			                 , STUFF(STUFF(MS.sdate, 5, 0, '-'), 8, 0, '-') AS SDATE 
	                         , STUFF(STUFF(MS.edate, 5, 0, '-'), 8, 0, '-') AS EDATE
	 		                 , DBO.F_U_WEEK_HAN(MS.week_day) AS WEEK_DAY
			                 , (TS.sdnm + '-' + view_sdnm) AS SDNM
                             , MS.sdno
                             , TS.bkno
			                 , MS.use_yn
			                 , MS.j_use_yn
			                 , MS.j_count
			                 , MS.j_hitpoint
			                 , MS.j_quiz_cnt
			                 , MS.correct_yn
			                 , MS.m_use_yn
			                 , MS.m_count			 
			                 , MS.m_hitpoint
			                 , MS.m_quiz_cnt
			                 , MS.m_quiz_type
			                 , MS.l_quiz_cnt
			                 , MS.concept_yn
			                 , MS.quiz_yn
			                 , MS.menu_yn
			                 , (SELECT usernm FROM tls_member WHERE userid = MS.rid) AS RID
			                 , MS.RDATETIME
			                 , (SELECT usernm FROM tls_member WHERE userid = MS.uid) AS UID
			                 , MS.UDATETIME
                             , MS.yyyy
                             , MS.cpno
                             , MS.userid
                             , TM.login_id
                             , TM.login_pwd
	                     FROM tls_member_study AS MS
                    LEFT JOIN tls_member AS TM
                           ON MS.userid = TM.userid
                    LEFT JOIN tls_class AS TC
	                       ON MS.cpno = TC.cpno and MS.clno = TC.clno
	                LEFT JOIN tls_study AS TS
	                       ON MS.sdno = TS.sdno
		                WHERE 1=1
                    ";
                    if (!string.IsNullOrEmpty(ClassEmployeeCPNO))
                    {
                        pSqlCommand.CommandText += @"
                         AND MS.cpno = '" + ClassEmployeeCPNO + "' ";
                    }
                    if (!string.IsNullOrEmpty(toolStripTextBoxClassNM2.Text))
                    {
                        pSqlCommand.CommandText += @"
                            AND TC.clnm LIKE '%" + toolStripTextBoxClassNM2.Text + "%' ";
                    }
                    if (!string.IsNullOrEmpty(toolStripTextBoxStudentNM2.Text))
                    {
                        pSqlCommand.CommandText += @"
                            AND TM.usernm LIKE '%" + toolStripTextBoxStudentNM2.Text + "%' ";
                    }
                    if (!string.IsNullOrEmpty(toolStripTextBoxStudyNM2.Text))
                    {
                        pSqlCommand.CommandText += @"
                            AND TS.sdnm LIKE '%" + toolStripTextBoxStudyNM2.Text + "%' ";
                    }                    
                    pSqlCommand.CommandText += @"                      
                            AND REPLACE(CONVERT(VARCHAR(10), '" + dateTimePickerStudentStudy.Value + @"', 112), '-', '') BETWEEN MS.sdate AND MS.edate		            
                        ORDER BY TC.clnm, usernm, MS.sdate
                    ";
                    toolStripTextBoxClassNM2.Text = "";
                    toolStripTextBoxStudentNM2.Text = "";
                    toolStripTextBoxStudyNM2.Text = "";
                    toolStripTextBoxStudentBookNM.Text = "";
                    toolStripTextBoxStudentDataTime.Text = "";
                    break;

                case "select_student_study_datatime_all":

                    //반별 학생, 학습명 차시 정보 조회(과정2) - 수업일 전체 조회
                    pSqlCommand.CommandText = @"                       
		                SELECT (SELECT usernm FROM tls_member WHERE userid = MS.tid) AS TID
	    	                 , (SELECT cpnm FROM tls_campus WHERE cpno = MS.cpno) AS CPNM
                             , MS.term_cd
		                     , TC.clnm
                             , TM.usernm
			                 , STUFF(STUFF(MS.sdate, 5, 0, '-'), 8, 0, '-') AS SDATE 
	                         , STUFF(STUFF(MS.edate, 5, 0, '-'), 8, 0, '-') AS EDATE
	 		                 , DBO.F_U_WEEK_HAN(MS.week_day) AS WEEK_DAY
			                 , (TS.sdnm + '-' + view_sdnm) AS SDNM
                             , MS.sdno
                             , TS.bkno
			                 , MS.use_yn
			                 , MS.j_use_yn
			                 , MS.j_count
			                 , MS.j_hitpoint
			                 , MS.j_quiz_cnt
			                 , MS.correct_yn
			                 , MS.m_use_yn
			                 , MS.m_count			 
			                 , MS.m_hitpoint
			                 , MS.m_quiz_cnt
			                 , MS.m_quiz_type
			                 , MS.l_quiz_cnt
			                 , MS.concept_yn
			                 , MS.quiz_yn
			                 , MS.menu_yn
			                 , (SELECT usernm FROM tls_member WHERE userid = MS.rid) AS RID
			                 , MS.RDATETIME
			                 , (SELECT usernm FROM tls_member WHERE userid = MS.uid) AS UID
			                 , MS.UDATETIME
                             , MS.yyyy
                             , MS.cpno
                             , MS.userid
                             , TM.login_id
                             , TM.login_pwd
	                     FROM tls_member_study AS MS
                    LEFT JOIN tls_member AS TM
                           ON MS.userid = TM.userid
                    LEFT JOIN tls_class AS TC
	                       ON MS.cpno = TC.cpno and MS.clno = TC.clno
	                LEFT JOIN tls_study AS TS
	                       ON MS.sdno = TS.sdno
		                WHERE 1=1
                    ";
                    if (!string.IsNullOrEmpty(ClassEmployeeCPNO))
                    {
                        pSqlCommand.CommandText += @"
                    AND MS.cpno = '" + ClassEmployeeCPNO + "' ";
                    }
                    if (!string.IsNullOrEmpty(toolStripTextBoxClassNM2.Text))
                    {
                        pSqlCommand.CommandText += @"
                            AND TC.clnm LIKE '%" + toolStripTextBoxClassNM2.Text + "%' ";
                    }
                    if (!string.IsNullOrEmpty(toolStripTextBoxStudentNM2.Text))
                    {
                        pSqlCommand.CommandText += @"
                            AND TM.usernm LIKE '%" + toolStripTextBoxStudentNM2.Text + "%' ";
                    }
                    if (!string.IsNullOrEmpty(toolStripTextBoxStudyNM2.Text))
                    {
                        pSqlCommand.CommandText += @"
                            AND TS.sdnm LIKE '%" + toolStripTextBoxStudyNM2.Text + "%' ";
                    }      
                    pSqlCommand.CommandText += @"                                                 
                        ORDER BY TC.clnm, usernm, MS.sdate DESC
                    ";
                    toolStripTextBoxClassNM2.Text = "";
                    toolStripTextBoxStudentNM2.Text = "";
                    toolStripTextBoxStudyNM2.Text = "";
                    toolStripTextBoxStudentBookNM.Text = "";
                    toolStripTextBoxStudentDataTime.Text = "";
                    break;

                case "select_student_schedule":

                    //학생 차시 리스트 
                    pSqlCommand.CommandText += @"
                        SELECT STUFF(STUFF(A.cdate, 5, 0, '-'), 8, 0, '-') AS CDATE
			                 , DBO.F_U_WEEK_HAN(A.cweek_cd) AS CWEEK_CD
			                 , CONVERT(VARCHAR(2), F.sort) + '단원' + ' ' +  CONVERT(VARCHAR(2), E.time_cnt) + '차시' AS TIME_CNT
			                 , B.bknm
			                 , D.sdnm + '-' + D.view_sdnm AS SDNM
			                 , (SELECT name1 FROM tbl_quiz_class WHERE classa = F.classa) AS CLASSA
			                 , G.view_unnm
			                 , H.view_dfnm AS DFNM
			                 , CASE G.room_code WHEN 'P' THEN 'SMART Room'
					 	                        WHEN 'I' THEN 'LESSON Room'
						                        WHEN 'S' THEN 'SDL Room'
			                   END AS ROOM_CODE
			                 , CASE G.study_type_1 WHEN 'PC' THEN 'PC-(동+문)'
						                           WHEN 'PM' THEN 'PM-(동+문)'
						                           WHEN 'PQ' THEN 'PQ-(동+문)'
						                           WHEN 'E' THEN '채점'
							                       WHEN 'R' THEN '단말기'
							                       WHEN 'Q' THEN '문제풀이'
							                       WHEN 'M' THEN '동영상'
			                   END AS study_type_1
			                 , CASE G.study_type_2 WHEN 'PC' THEN 'PC-(동+문)'
							                       WHEN 'PM' THEN 'PM-(동+문)'
							                       WHEN 'PQ' THEN 'PQ-(동+문)'
							                       WHEN 'E' THEN '채점'
							                       WHEN 'R' THEN '단말기'
							                       WHEN 'Q' THEN '문제풀이'
							                       WHEN 'M' THEN '동영상'
			                   END AS study_type_2			                         		
		                  FROM tls_member_schedule AS A
                    INNER JOIN tls_book AS B 
	                        ON B.bkno = A.bkno
                    INNER JOIN tls_study AS D 
                            ON D.sdno = A.sdno
                    INNER JOIN tls_schedule AS E 
		                    ON E.sdno = A.sdno AND E.scno = A.scno
                    INNER JOIN TLS_CHAPTER AS F 
		                    ON F.bkno = B.bkno AND F.chno = E.chno
                    INNER JOIN tls_unit AS G 
		                    ON G.bkno = B.bkno AND G.chno = F.chno AND G.unno = E.unno
                    INNER JOIN tls_lvl_def AS H 
		                    ON H.lvno = G.lvno AND H.dfno = G.dfno
	                     WHERE A.yyyy = '" + this._common.GetCellValue(dataGridViewStudentStudy, dataGridViewStudentStudy.CurrentCell.RowIndex, "yyyy") + @"'
	                       AND A.term_cd = '" + this._common.GetCellValue(dataGridViewStudentStudy, dataGridViewStudentStudy.CurrentCell.RowIndex, "term_cd") + @"'
		                   AND A.cpno = '" + this._common.GetCellValue(dataGridViewStudentStudy, dataGridViewStudentStudy.CurrentCell.RowIndex, "cpno") + @"'
		                   AND A.userid = '" + this._common.GetCellValue(dataGridViewStudentStudy, dataGridViewStudentStudy.CurrentCell.RowIndex, "userid") + @"'
		                   AND A.sdno = '" + this._common.GetCellValue(dataGridViewStudentStudy, dataGridViewStudentStudy.CurrentCell.RowIndex, "sdno") + @"'
                    ";
                    if (!string.IsNullOrEmpty(toolStripTextBoxStudentBookNM.Text))
                    {
                        pSqlCommand.CommandText += @"
                            AND G.view_unnm LIKE '%" + toolStripTextBoxStudentBookNM.Text + "%' ";
                    }
                    if (!string.IsNullOrEmpty(toolStripTextBoxStudentDataTime.Text))
                    {
                        pSqlCommand.CommandText += @"
                            AND A.cdate LIKE '" + toolStripTextBoxStudentDataTime.Text + "%' ";
                    }
                    pSqlCommand.CommandText += @"
	                     ORDER BY cdate, G.sort
                    ";
                    toolStripTextBoxStudentBookNM.Text = "";
                    toolStripTextBoxStudentDataTime.Text = "";
                    break;
                default:
                    break;
            }

            return pSqlCommand;
        }

        #endregion Method

        #region Event

        private void UserControlStudy_Load(object sender, EventArgs e)
        {
            InitCombo();

            if (StudyType != null)
            {
                Select();

            }

        }

        public void Select(string param1 = "", string param2 = "", string param3 = "", string param4 = "", string param5 = "", string param6 = "", string param7 = "")
        {
            if (param1 != null)
            {
                StudyType = param1;
                ClassEmployeeCPNO = param2;
                ClassEmployeeCLNO = param3;
                ClassStudentCPNO = param4;
                ClassStudentUID = param5;
                ClassEmployeeUID = param6;
                ClassSchoolCDStudy = param7;
            }

            switch (StudyType)
            {
                case "C": //반 차시 조회
                    tabControl1.SelectedTab = tabPageClassSchedule;
                    SelectDataGridView(dataGridViewClassStudy, "select_class_study");
                    if (dataGridViewClassStudy.Rows.Count > 0 && dataGridViewClassStudy.CurrentCell != null)
                    {
                        //toolStripTextBoxClassNM.Text = this._common.GetCellValue(dataGridViewClassStudy, dataGridViewClassStudy.CurrentCell.RowIndex, "clnm");
                        //toolStripTextBoxClassTID.Text = this._common.GetCellValue(dataGridViewClassStudy, dataGridViewClassStudy.CurrentCell.RowIndex, "tid");
                    }
                    
                    break;
                case "S": //학생 차시 조회
                    tabControl1.SelectedTab = tabPageStudentSchedule;
                    SelectDataGridView(dataGridViewStudentStudy, "select_student_study");
                    //toolStripTextBoxClassNM2.Text = this._common.GetCellValue(dataGridViewStudentStudy, dataGridViewStudentStudy.CurrentCell.RowIndex, "clnm");
                    //toolStripTextBoxStudentNM2.Text = this._common.GetCellValue(dataGridViewStudentStudy, dataGridViewStudentStudy.CurrentCell.RowIndex, "usernm");
                    break;
                case "N": //차시를 조회하지 않는다.                    
                    toolStripTextBoxClassNM.Text = "";
                    toolStripTextBoxClassTID.Text = "";
                    toolStripTextBoxClassBookNM.Text = "";
                    toolStripTextBoxClassDataTime.Text = "";
                    toolStripTextBoxClassNM2.Text = "";
                    toolStripTextBoxStudentNM2.Text = "";
                    toolStripTextBoxStudentBookNM.Text = "";
                    toolStripTextBoxStudentDataTime.Text = "";
                    dataGridViewClassStudy.Rows.Clear();
                    dataGridViewClassSchedule.Rows.Clear();
                    dataGridViewStudentStudy.Rows.Clear();
                    dataGridViewStudentSchedule.Rows.Clear();
                    break;
                default:
                    break;
            }
        }
        private void dataGridViewStudentStudy_MouseClick(object sender, MouseEventArgs e)
        {
            //과정2 학생 u2m학습창 및 마이페이지 로그인
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
                if (dataGridViewStudentStudy.Rows.Count > 0 && dataGridViewStudentStudy.CurrentCell != null)
                {
                    //과정2 텍스트 박스 반명, 학생명 표시
                    toolStripTextBoxClassNM2.Text = this._common.GetCellValue(dataGridViewStudentStudy, dataGridViewStudentStudy.CurrentCell.RowIndex, "clnm");
                    //toolStripTextBoxStudentNM2.Text = this._common.GetCellValue(dataGridViewStudentStudy, dataGridViewStudentStudy.CurrentCell.RowIndex, "usernm");                
                }
            }
        }
        private void dataGridViewClassStudy_MouseClick(object sender, MouseEventArgs e)
        {
            
            
                if (e.Button == MouseButtons.Left)
                {
                    if (dataGridViewClassStudy.Rows.Count > 0 && dataGridViewClassStudy.CurrentCell != null)
                    {
                        //과정1 텍스트 박스 반명, 학생명 표시
                        toolStripTextBoxClassNM.Text = this._common.GetCellValue(dataGridViewClassStudy, dataGridViewClassStudy.CurrentCell.RowIndex, "clnm");
                        //toolStripTextBoxClassTID.Text = this._common.GetCellValue(dataGridViewClassStudy, dataGridViewClassStudy.CurrentCell.RowIndex, "tid");
                    }
                }
        }
        private void toolStripTextBoxClassNM_KeyDown(object sender, KeyEventArgs e)
        {
            //과정1 반별 차시 조회
            if (e.KeyCode == Keys.Enter)
            {
                SelectDataGridView(dataGridViewClassStudy, "select_class_study_all");
            }
        }

        private void toolStripTextBoxStudyNM_KeyDown(object sender, KeyEventArgs e)
        {
            //과정1 학습명별 차시 조회
            if (e.KeyCode == Keys.Enter)
            {
                SelectDataGridView(dataGridViewClassStudy, "select_class_study_all");
            }
        }
        private void toolStripTextBoxClassTID_KeyDown(object sender, KeyEventArgs e)
        {
            //과정1 수업교사 차시 조회
            if (e.KeyCode == Keys.Enter)
            {
                SelectDataGridView(dataGridViewClassStudy, "select_class_study_all");
            }
        }
        private void buttonClassStudy_Click(object sender, EventArgs e)
        {
            //과정1 수업일 기준 차시 조회
            SelectDataGridView(dataGridViewClassStudy, "select_class_study_all");
        }

        private void buttonClassStudyAll_Click(object sender, EventArgs e)
        {
            //과정1 수업일 전체 차시 조회
            SelectDataGridView(dataGridViewClassStudy, "select_class_study_datatime_all");
        }
        private void dataGridViewClassStudy_DoubleClick(object sender, EventArgs e)
        {
            //과정1 차시 리스트 조회
            if (dataGridViewClassStudy.Rows.Count > 0 && dataGridViewClassStudy.CurrentCell != null)
            {
                SelectDataGridView(dataGridViewClassSchedule, "select_class_schedule");
            }
        }
        private void toolStripTextBoxClassBookNM_KeyDown(object sender, KeyEventArgs e)
        {
            //과정1 교재구성명별 차시 리스트 조회
            if (e.KeyCode == Keys.Enter)
            {
                if (dataGridViewClassSchedule.Rows.Count > 0 && dataGridViewClassSchedule.CurrentCell != null)
                {
                    SelectDataGridView(dataGridViewClassSchedule, "select_class_schedule");
                }                
            }
        }
        private void dataGridViewClassSchedule_Click(object sender, EventArgs e)
        {
            //과정1 교재구성명 텍스트 박스 표시  
            if (dataGridViewClassSchedule.Rows.Count > 0 && dataGridViewClassSchedule.CurrentCell != null)
            {
                toolStripTextBoxClassBookNM.Text = this._common.GetCellValue(dataGridViewClassSchedule, dataGridViewClassSchedule.CurrentCell.RowIndex, "view_unnm");
            }
        }
        private void toolStripTextBoxClassDataTime_KeyDown(object sender, KeyEventArgs e)
        {
            //과정1 수업일 기준 차시 리스트 조회
            if (e.KeyCode == Keys.Enter)
            {
                if (dataGridViewClassSchedule.Rows.Count > 0 && dataGridViewClassSchedule.CurrentCell != null)
                {
                    SelectDataGridView(dataGridViewClassSchedule, "select_class_schedule");
                }                
            }
        }
        private void toolStripTextBoxClassNM2_KeyDown(object sender, KeyEventArgs e)
        {
            //과정2 해당반 모든학생 차시 조회
            if (e.KeyCode == Keys.Enter)
            {
                SelectDataGridView(dataGridViewStudentStudy, "select_student_study_all");
            }
        }
        private void toolStripTextBoxStudentNM2_KeyDown(object sender, KeyEventArgs e)
        {
            //과정2 학생별 차시 조회
            if (e.KeyCode == Keys.Enter)
            {
                SelectDataGridView(dataGridViewStudentStudy, "select_student_study_all");
            }
        }

        private void toolStripTextBoxStudyNM2_KeyDown(object sender, KeyEventArgs e)
        {
            //과정2 학습명별 차시 조회
            if (e.KeyCode == Keys.Enter)
            {
                SelectDataGridView(dataGridViewStudentStudy, "select_student_study_all");
            }
        }
        private void buttonStudentStudy_Click(object sender, EventArgs e)
        {
            //과정2 수업일 기준 차시 조회
            SelectDataGridView(dataGridViewStudentStudy, "select_student_study_all");
        }
        private void buttonStudentStudyAll_Click(object sender, EventArgs e)
        {
            ////과정2 수업일 전체 차시 조회
            SelectDataGridView(dataGridViewStudentStudy, "select_student_study_datatime_all");
        }
        private void dataGridViewStudentStudy_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridViewStudentStudy.Rows.Count > 0 && dataGridViewStudentStudy.CurrentCell != null)
            {
                //과정2 차시 리스트 조회
                SelectDataGridView(dataGridViewStudentSchedule, "select_student_schedule");
            }
        }
        private void toolStripTextBoxStudentBookNM_KeyDown(object sender, KeyEventArgs e)
        {
            //과정2 교재구성명별 차시 리스트 조회
            if (e.KeyCode == Keys.Enter)
            {
                if (dataGridViewStudentSchedule.Rows.Count > 0 && dataGridViewStudentSchedule.CurrentCell != null)
                {
                    SelectDataGridView(dataGridViewStudentSchedule, "select_student_schedule");
                }                
            }
        }
        private void dataGridViewStudentSchedule_Click(object sender, EventArgs e)
        {
            //과정2 교재구성명 텍스트 박스 표시  
            if (dataGridViewStudentSchedule.Rows.Count > 0 && dataGridViewStudentSchedule.CurrentCell != null)
            {
                toolStripTextBoxStudentBookNM.Text = this._common.GetCellValue(dataGridViewStudentSchedule, dataGridViewStudentSchedule.CurrentCell.RowIndex, "view_unnm");
            }
        }
        private void toolStripTextBoxStudentDataTime_KeyDown(object sender, KeyEventArgs e)
        {
            //과정2 수업일 기준 차시 리스트 조회
            if (e.KeyCode == Keys.Enter)
            {
                if (dataGridViewStudentSchedule.Rows.Count > 0 && dataGridViewStudentSchedule.CurrentCell != null)
                {
                    SelectDataGridView(dataGridViewStudentSchedule, "select_student_schedule");
                }                
            }
        }
        #endregion Event

        
        
        private void dataGridViewStudentSchedule_DoubleClick(object sender, EventArgs e)
        {
            
                toolStripTextBoxStudentBookNM.Text = this._common.GetCellValue(dataGridViewStudentSchedule, dataGridViewStudentSchedule.CurrentCell.RowIndex, "view_unnm");
                SelectDataGridView(dataGridViewStudentSchedule, "select_student_schedule");
            
        }

        

























    }
}
