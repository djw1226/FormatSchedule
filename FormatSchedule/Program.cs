
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.Diagnostics.Tracing;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using excelObj = Microsoft.Office.Interop.Excel;

namespace FormatSchedule
{
    class Program
    {
        static private SportsEngineController _controller;
        static private DatabaseContext _context;
        
        static void Main(string[] args)
        {
            string fileName = args[0];
            _controller = new SportsEngineController();
            _context = new DatabaseContext();
            //CreateEvent();

            ProcessSchedule(fileName);




        }
        static private void ProcessSchedule(string fileName)
        {
            excelObj.Application myExcel = new excelObj.Application();

            excelObj.Workbooks workbooks = myExcel.Workbooks;

            excelObj.Workbook schedule = workbooks.Open(fileName);
            excelObj.Worksheet sheet = (excelObj.Worksheet)schedule.Worksheets[1];
            excelObj.Workbook template = workbooks.Open("c:\\BaseballScheduler\\Upload_Template_Empty.csv");
            excelObj.Worksheet destination = (excelObj.Worksheet)template.Worksheets[1];

            excelObj.Range last = sheet.Cells.SpecialCells(excelObj.XlCellType.xlCellTypeLastCell, Type.Missing);
            int rowCount = last.Row;
            for (int i = 2; i <= rowCount; i++)
            {
                
                excelObj.Range sourceRange;
                LeagueAthleticEvent leagueEvent = new LeagueAthleticEvent();
                sourceRange = (excelObj.Range)sheet.Cells[i, 6];
                leagueEvent.Location = sourceRange.Value;
                sourceRange = (excelObj.Range)sheet.Cells[i, 2];
                DateTime eventDate = (DateTime) sourceRange.Value;
                sourceRange = (excelObj.Range)sheet.Cells[i, 3];
                string startHour = DateTime.FromOADate(sourceRange.Value).ToString("HH:mm");
                sourceRange = (excelObj.Range)sheet.Cells[i, 4];
                string endHour = DateTime.FromOADate(sourceRange.Value).ToString("HH:mm");
                leagueEvent.StartDateTime = eventDate.ToString("yyyy-MM-dd") + "T" + startHour + "-5:00";
                leagueEvent.EndDateTime = eventDate.ToString("yyyy-MM-dd") + "T" + endHour + "-5:00";
                sourceRange = (excelObj.Range)sheet.Cells[i, 5];
                string teamCode = sourceRange.Value.ToString();
                Team team = GetTeam(teamCode);
                if (team == null)
                {
                    continue;
                }
                leagueEvent.PageNodeId = team.PageNodeId;
                sourceRange = (excelObj.Range)sheet.Cells[i, 13];
                leagueEvent.LeagueAthleticID = int.Parse(sourceRange.Value.ToString());
                //type
                sourceRange = (excelObj.Range)sheet.Cells[i, 10];
                string status = "";
                if (sourceRange.Value != null)
                {
                    status = sourceRange.Value;
                }
                if (status == "CANCELLED")
                {
                    CancelEvent(leagueEvent, eventDate);
                    continue;
                }

                sourceRange = (excelObj.Range)sheet.Cells[i, 7];
                leagueEvent.Status = "scheduled";
                if (sourceRange.Value == null || sourceRange.Value.ToString() != "Game")
                {
                    leagueEvent.Title = "Practice";
                    leagueEvent.EventType = "practice";
                    CreateEvent(leagueEvent);
                }
                else
                {
                    //get opponent
                    sourceRange = (excelObj.Range)sheet.Cells[i, 8];
                    if (sourceRange.Value == null)
                    {
                        //travel game
                        leagueEvent.Title = "League Athletics Game";
                        leagueEvent.EventType = "game";
                        CreateEvent(leagueEvent);
                    }
                    else
                    { 
                        string opponentCode = sourceRange.Value;
                        Team opponent = GetTeam(opponentCode);
                        if (opponent == null)
                        {
                            continue;
                        }
                        leagueEvent.Team1 = team.TeamId;
                        leagueEvent.Team2 = opponent.TeamId;
                        leagueEvent.Team2PageNodeId = opponent.PageNodeId;
                        leagueEvent.Title = "Game " + opponent.TeamCode + " at " + team.TeamCode;
                        leagueEvent.EventType = "game";
                        if (status == "CANCELLED")
                        {
                            CancelGame(leagueEvent, eventDate);
                            continue;
                        }
                        //CreateGame(leagueEvent);
                        CreateEvent(leagueEvent);

                    }

                }
            }

           
            template.Close(false);
            schedule.Close(false);
            System.Runtime.InteropServices.Marshal.ReleaseComObject(template);
            template = null;
            System.Runtime.InteropServices.Marshal.ReleaseComObject(schedule);
            schedule = null;
            System.Runtime.InteropServices.Marshal.ReleaseComObject(workbooks);
            workbooks = null;

            myExcel.Quit();
            System.Runtime.InteropServices.Marshal.ReleaseComObject(myExcel);
            myExcel = null;
            Environment.Exit(0);
        }

        private static void CancelEvent(LeagueAthleticEvent leagueEvent, DateTime eventDate)
        {
            if (eventDate == DateTime.Today)
            {
                leagueEvent.Status = "CANCELLED";
                CreateEvent(leagueEvent);
            }
            else
            {
                string sportsEngineId;
                var query = _context.Events.Where(e => e.LeagueAthleticsID == leagueEvent.LeagueAthleticID).FirstOrDefault<Event>();
                if (query != null)
                {
                    sportsEngineId = query.SportsEngineID;
                    sportsEngineId = _controller.CancelEvent(leagueEvent, sportsEngineId);
                    _context.Events.Remove(query);
                }

                

            }
        }

        private static void CancelGame(LeagueAthleticEvent leagueEvent, DateTime eventDate)
        {
            if (eventDate == DateTime.Today)
            {
                leagueEvent.Status = "CANCELLED";
                CreateGame(leagueEvent);
            }
            else
            {
                string sportsEngineId;
                var query = _context.Events.Where(e => e.LeagueAthleticsID == leagueEvent.LeagueAthleticID).FirstOrDefault<Event>();
                if (query != null)
                {
                    sportsEngineId = query.SportsEngineID;
                    sportsEngineId = _controller.CancelGame(sportsEngineId);
                    _context.Events.Remove(query);
                }



            }
        }

        static private void CreateEvent(LeagueAthleticEvent leagueEvent)
        {
            string json = "{\"event\":{" +
               "\"title\": \"" + leagueEvent.Title + "\"," +
               "\"page_node_ids\":" + leagueEvent.PageNodeId.ToString() + "," +
               "\"start_date_time\":\"" + leagueEvent.StartDateTime + "\"," +
               "\"end_date_time\":\"" + leagueEvent.EndDateTime + "\"," +
               "\"event_type\":\"" + leagueEvent.EventType + "\"," +
               "\"status\":\"" + leagueEvent.Status + "\"," +
               "\"location\":\"" + leagueEvent.Location + "\"}}";
            if (leagueEvent.Team2PageNodeId > 0)
            {
                json = "{\"event\":{" +
               "\"title\": \"" + leagueEvent.Title + "\"," +
               "\"page_node_ids\":[" + leagueEvent.PageNodeId.ToString() + "," + leagueEvent.Team2PageNodeId.ToString() + "]," +
               "\"start_date_time\":\"" + leagueEvent.StartDateTime + "\"," +
               "\"end_date_time\":\"" + leagueEvent.EndDateTime + "\"," +
               "\"event_type\":\"" + leagueEvent.EventType + "\"," +
               "\"status\":\"" + leagueEvent.Status + "\"," +
               "\"location\":\"" + leagueEvent.Location + "\"}}";
            }
            string sportsEngineId;
            var query = _context.Events.Where(e => e.LeagueAthleticsID == leagueEvent.LeagueAthleticID).FirstOrDefault<Event>();
            if (query == null)
            {
                sportsEngineId = _controller.CreateEvent(leagueEvent, json);
                Event seEvent = new Event();
                seEvent.SportsEngineID = sportsEngineId;
                seEvent.LeagueAthleticsID = leagueEvent.LeagueAthleticID;
                _context.Events.Add(seEvent);
                _context.SaveChanges();

            }
            else
            {
                sportsEngineId = query.SportsEngineID;
                sportsEngineId = _controller.UpdateEvent(leagueEvent, sportsEngineId, json);
            }
        }

        static private void CreateGame(LeagueAthleticEvent leagueEvent)
        {
            string json = "{\"game\":{" +
               "\"team_1_id\": \"" + leagueEvent.Team1.ToString() + "\"," +
               "\"team_2_id\":\"" + leagueEvent.Team2.ToString() + "\"," +
               "\"start_date_time\":\"" + leagueEvent.StartDateTime + "\"," +
               "\"end_date_time\":\"" + leagueEvent.EndDateTime + "\"," +
               "\"subseason_id\":\"" + ConfigurationManager.AppSettings["InHouseSubseasonId"] + "\"," +
               "\"sport_id\":\"" + ConfigurationManager.AppSettings["SportId"] + "\"," +
               "\"status\":\"" + leagueEvent.Status + "\"," +
               "\"team_1_is_home\":\"true\"," + 
               "\"location\":\"" + leagueEvent.Location + "\"}}";
            string sportsEngineId;
            var query = _context.Events.Where(e => e.LeagueAthleticsID == leagueEvent.LeagueAthleticID).FirstOrDefault<Event>();
            if (query == null)
            {
                sportsEngineId = _controller.CreateGame(json);
                Event seEvent = new Event();
                seEvent.SportsEngineID = sportsEngineId;
                seEvent.LeagueAthleticsID = leagueEvent.LeagueAthleticID;
                _context.Events.Add(seEvent);
                _context.SaveChanges();

            }
            else
            {
                sportsEngineId = query.SportsEngineID;
                sportsEngineId = _controller.UpdateGame(sportsEngineId, json);
            }
        }

        static private Team GetTeam(string teamCode)
        {
            Team team = _context.Teams.Where(t => t.TeamCode == teamCode).FirstOrDefault<Team>();
            return team;
        }
    }
}
