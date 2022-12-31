using LogisticHelper.Models;
using LogisticHelper.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using ServiceReference1;
using System.IO.Compression;
using System.Xml;
using Quartz;
using Quartz.Impl;

namespace LogisticHelper.Controllers
{
    public class TercController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        
        public TercController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
          

        }
        // GET: BookCoverController
        
        public  IActionResult Index()
        {

            IEnumerable<Terc> objUserList =  _unitOfWork.Terc.GetAll();
            return  View(objUserList);
        }



        public IActionResult Search()
        {

            IEnumerable<Terc> objUserList = _unitOfWork.Terc.GetAll();

            return View(objUserList);
        }  
        public TerytWs1Client connection()
        {
            ServiceReference1.TerytWs1Client client = new ServiceReference1.TerytWs1Client();
            /*  serviceteryt.TerytWs1Client client = new serviceteryt.TerytWs1Client();*/


            client.ClientCredentials.UserName.UserName = "Mariusz.Sobota";
            client.ClientCredentials.UserName.Password = "so6QT8ahG";
            client.OpenAsync().Wait();
            return client;
        }
        //AUTOCOMPLETE FUNCTION FOR SEARCH PURPOSES
        //Function allows us to search data from table Terc, thanks to this,
        //we can find Gmina, Powiat and Wojewodztwo, just by  typing in textbox
        [HttpPost]
        public JsonResult AutoComplete(string input)
        {
            TerytWs1Client client = connection();

            //create a list of all Terc elements
            
            IEnumerable<Terc> objTercList = _unitOfWork.Terc.GetAll();
            //scan them
            var search = (from Terc in objTercList
                          where
                           Terc.NAZWA.StartsWith(input)
                          select new
                          {

                              label = Terc.NAZWA,
                              val =Terc.WOJ + Terc.POW + Terc.GMI + Terc.RODZ,

                          }).Take(5).ToList();
            return Json(search);
        }

        ///Function which allows to connect to TERYT API, returns TerytWS1Client 
        ///which allows co operate on data stored inside
     

        //Function which allows for checking value of nodes

        string handleNull(string xAfter, string xBefore)
        {
            if (xAfter == "")
                xAfter = xBefore;
            return xAfter;

        }


        ///Add Jednostka Administracyjna which contains all data
        Terc addJA(string xWoj, string xPow, string xGmi, string xRodz, string xNazwa, string xNazwaDodatkowa, string xStan)
        {
            var additionQuery = new Terc
            {
                WOJ = xWoj,
                POW = xPow,
                GMI = xGmi,
                RODZ = xRodz,
                NAZWA = xNazwa,
                NAZWA_DOD = xNazwaDodatkowa,
                STAN_NA = xStan
            };
            _unitOfWork.Terc.Add(additionQuery);
            _unitOfWork.Save();
            return additionQuery;

            ///Polymorph version in case of powiat addition
        }
        Terc addJA(string xWoj, string xPow, string xNazwa, string xNazwaDodatkowa, string xStan)
        {
            var additionQuery = new Terc
            {
                WOJ = xWoj,
                POW = xPow,
                NAZWA = xNazwa,
                NAZWA_DOD = xNazwaDodatkowa,
                STAN_NA = xStan
            };
            _unitOfWork.Terc.Add(additionQuery);
            _unitOfWork.Save();
            return additionQuery;
        }
        ///Polymorph version in case of wojewodztwo addition
        Terc addJA(string xWoj, string xNazwa, string xNazwaDodatkowa, string xStan)
        {
            var additionQuery = new Terc
            {
                WOJ = xWoj,

                NAZWA = xNazwa,
                NAZWA_DOD = xNazwaDodatkowa,
                STAN_NA = xStan
            };
            _unitOfWork.Terc.Add(additionQuery);
            _unitOfWork.Save();
            return additionQuery;
        }

      

        [HttpPost]
        public object Search(string search)
        {
            Schedule();

            return View();
        }


      

        public void Schedule()
        {
            TerytWs1Client client = connection();

            

            IEnumerable<Terc> TercObjList = _unitOfWork.Terc.GetAll();


            //Teec is updated once a year

            DateTime s = DateTime.Now;
            string startingDate = s.ToString("yyyy-MM-dd");


            DateTime e = s.AddYears(-1);
            string endingDate = e.ToString("yyyy-MM-dd");



            //Here we have XML compressed to ZIP, now figure out how to suck it to db
            //FileChange is a variable in which is file, it doesnt exist phyisically on disc, how to unzip it?
            var UpdateFile = client.PobierzZmianyTercUrzedowyAsync(e, s);
            PlikZmiany fileChange = UpdateFile.Result;
            string fileName = fileChange.nazwa_pliku;
            string zipContent = fileChange.plik_zawartosc;
            string scenario = fileChange.opis;

            //working decoding from base64 to zip
            Chilkat.BinData zipData = new Chilkat.BinData();
            bool success = zipData.AppendEncoded(zipContent, "base64");
            success = zipData.WriteFile(Directory.GetCurrentDirectory() + @"/File/out.zip");





            FileStream fs = new FileStream("./File/out.zip", FileMode.Open);
            ZipArchive zipArchive = new ZipArchive(fs);
            string destination = Directory.GetCurrentDirectory() + @"/File/";
            zipArchive.ExtractToDirectory(destination);
            ViewBag.Message = "Selected SS Name: " + zipContent;
            //  ViewBag.Message = "Selected GMI Name: " + search;

            //Above works, need smth to read xml


            XmlDocument doc = new XmlDocument();

            doc.Load(Directory.GetCurrentDirectory() + "/File/TERC_Urzedowy_zmiany_" + endingDate + "_" + startingDate + ".xml");
            var xList = doc.SelectNodes("/zmiany/zmiana"); // Znajdź węzeł zmiany, w której znajdują się informacje dot. modernizacji
            foreach (XmlNode xNode in xList)
            {
                var xTypKorekty = xNode.SelectSingleNode("TypKorekty");
                switch (xTypKorekty.InnerText)
                {
                    //Dodanie jednostki administracyjnej
                    case "D":
                        string xWojPo = (xNode.SelectSingleNode("WojPo").InnerText);
                        string xPowPo = (xNode.SelectSingleNode("PowPo").InnerText);
                        string xGmiPo = (xNode.SelectSingleNode("GmiPo").InnerText);
                        string xRodzPo = (xNode.SelectSingleNode("RodzPo").InnerText);
                        string xNazwaPo = xNode.SelectSingleNode("NazwaPo").InnerText;
                        string xNazwaDodatkowaPo = xNode.SelectSingleNode("NazwaDodatkowaPo").InnerText;
                        string xStanPo = (xNode.SelectSingleNode("StanPo").InnerText);

                        if (xNazwaDodatkowaPo.StartsWith("gmina") || xNazwaDodatkowaPo.Contains("miasto") || xNazwaDodatkowaPo.Contains("obszar"))
                        {
                            var queryCheck = (from toj in TercObjList where toj.WOJ == xWojPo && toj.POW == xPowPo && toj.GMI == xGmiPo && toj.RODZ == xRodzPo && toj.NAZWA == xNazwaPo select toj).FirstOrDefault();
                            if (queryCheck != null)
                            {
                                break;
                            }
                            else
                            {

                                var queryAdd = addJA(xWojPo, xPowPo, xGmiPo, xRodzPo, xNazwaPo, xNazwaDodatkowaPo, xStanPo);

                                break;

                            }

                        }
                        else if (xNazwaDodatkowaPo.Contains("powiat"))
                        {
                            var queryCheck = (from toj in TercObjList where toj.WOJ == xWojPo && toj.POW == xPowPo && toj.NAZWA == xNazwaPo select toj).FirstOrDefault();
                            if (queryCheck != null)
                            {
                                break;
                            }
                            else
                            {

                                var queryAdd = addJA(xWojPo, xPowPo, xNazwaPo, xNazwaDodatkowaPo, xStanPo);

                                break;

                            }
                        }
                        else if (xNazwaDodatkowaPo.Contains("wojewodztwo"))
                        {
                            var queryCheck = (from toj in TercObjList where toj.WOJ == xWojPo && toj.NAZWA == xNazwaPo select toj).FirstOrDefault();
                            if (queryCheck != null)
                            {
                                break;
                            }
                            else
                            {

                                var queryAdd = addJA(xWojPo, xPowPo, xNazwaPo, xNazwaDodatkowaPo, xStanPo);

                                break;

                            }
                        }
                        break;





                    //Delete old Jednostka Administracyjna
                    case "U":

                        var xWojPrzed = (xNode.SelectSingleNode("WojPrzed").InnerText);
                        var xPowPrzed = (xNode.SelectSingleNode("PowPrzed").InnerText);
                        var xGmiPrzed = (xNode.SelectSingleNode("GmiPrzed").InnerText);
                        var xRodzPrzed = (xNode.SelectSingleNode("RodzPrzed").InnerText);
                        var xNazwaPrzed = xNode.SelectSingleNode("NazwaPrzed").InnerText;
                        var xNazwaDodatkowaPrzed = xNode.SelectSingleNode("NazwaDodatkowaPrzed").InnerText;
                        var xStanPrzed = xNode.SelectSingleNode("Stan_Na").InnerText;


                        var query = (from toj in TercObjList where toj.WOJ == xWojPrzed && toj.POW == xPowPrzed && toj.GMI == xGmiPrzed && toj.RODZ == xRodzPrzed && toj.NAZWA == xNazwaPrzed select toj).FirstOrDefault();
                        //check if value was not changed before
                        if (query == null)
                            break;

                        _unitOfWork.Terc.Remove(query);
                        _unitOfWork.Save();

                        break;

                    default:
                        break;


                    //Update current Jednostka Administracyjne
                    case "M":
                        xWojPrzed = (xNode.SelectSingleNode("WojPrzed").InnerText);
                        xPowPrzed = ((xNode.SelectSingleNode("PowPrzed").InnerText));
                        xGmiPrzed = (xNode.SelectSingleNode("GmiPrzed").InnerText);
                        xRodzPrzed = (xNode.SelectSingleNode("RodzPrzed").InnerText);
                        xNazwaPrzed = xNode.SelectSingleNode("NazwaPrzed").InnerText;
                        xNazwaDodatkowaPrzed = xNode.SelectSingleNode("NazwaDodatkowaPrzed").InnerText;
                        xStanPrzed = xNode.SelectSingleNode("StanPrzed").InnerText;



                        xWojPo = handleNull((xNode.SelectSingleNode("WojPo").InnerText), xWojPrzed);
                        xPowPo = handleNull((xNode.SelectSingleNode("PowPo").InnerText), xPowPrzed);

                        xGmiPo = handleNull((xNode.SelectSingleNode("GmiPo").InnerText), xGmiPrzed);
                        xRodzPo = handleNull((xNode.SelectSingleNode("RodzPo").InnerText), xRodzPrzed);
                        xNazwaPo = handleNull(xNode.SelectSingleNode("NazwaPo").InnerText, xNazwaPrzed);
                        xNazwaDodatkowaPo = handleNull(xNode.SelectSingleNode("NazwaDodatkowaPo").InnerText, xNazwaDodatkowaPrzed);
                        xStanPo = (xNode.SelectSingleNode("StanPo").InnerText);


                        // etc

                        // Jak zrobić żeby ignorował wartości NULL ??

                        //How to use string as requirement?
                        if (xNazwaDodatkowaPrzed.StartsWith("gmina") || xNazwaDodatkowaPrzed.Contains("miasto"))
                        {
                            query = (from toj in TercObjList where toj.WOJ == xWojPrzed && toj.POW != null && toj.POW == xPowPrzed && toj.GMI == xGmiPrzed && toj.RODZ == xRodzPrzed && toj.NAZWA == xNazwaPrzed select toj).FirstOrDefault();

                            //check if value was not changed before
                            if (query == null)
                                break;

                            query.WOJ = xWojPo;
                            query.POW = xPowPo;
                            query.GMI = xGmiPo;
                            query.RODZ = xRodzPo;
                            query.NAZWA = xNazwaPo;
                            query.NAZWA_DOD = xNazwaDodatkowaPo;
                            query.STAN_NA = xStanPo;

                            _unitOfWork.Terc.Update(query);
                        }
                        else if (xNazwaDodatkowaPrzed.StartsWith("powiat"))
                        {
                            query = (from toj in TercObjList where toj.WOJ == xWojPrzed && toj.POW == xPowPrzed && toj.NAZWA == xNazwaPrzed select toj).FirstOrDefault();

                            //check if value was not changed before
                            if (query == null)
                                break;

                            query.WOJ = xWojPo;
                            query.POW = xPowPo;
                            query.NAZWA = xNazwaPo;
                            query.NAZWA_DOD = xNazwaDodatkowaPo;
                            query.STAN_NA = xStanPo;

                            _unitOfWork.Terc.Update(query);
                        }
                        else if (xNazwaDodatkowaPrzed.StartsWith("województwo"))
                        {

                            query = (from toj in TercObjList where toj.WOJ == xWojPrzed && toj.NAZWA == xNazwaPrzed select toj).FirstOrDefault();
                            //check if value was not changed before
                            if (query == null)
                                break;

                            query.WOJ = xWojPo;

                            query.NAZWA = xNazwaPo;
                            query.NAZWA_DOD = xNazwaDodatkowaPo;
                            query.STAN_NA = xStanPo;

                            _unitOfWork.Terc.Update(query);

                        }

                        _unitOfWork.Save();
                        break;


                        //WORKS, now polishing this little boy!!!

                }

                // wezły


            }


          


        } 
      /*  public async Task<IActionResult> TercUpdateJob()
            {
                IJobDetail job = JobBuilder.Create<TercUpdateJob>()
                                            .WithIdentity("tercUpdateJob", "tercUpdate")
                                            .Build();
            ITrigger triggerUpdateTerc = TriggerBuilder.Create()
                                                        .WithIdentity("updateTercTrigger", "quartzTriggers")
                                                        .StartNow()
                                                        .WithSimpleSchedule(x => x.WithIntervalInHours(8742))
                                                        .Build();
            await _scheduler.ScheduleJob(job, triggerUpdateTerc);
                return RedirectToAction("Schedule");
            }
*/
    }
}