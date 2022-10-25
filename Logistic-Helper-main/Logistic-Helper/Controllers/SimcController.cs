using LogisticHelper.Models;
using LogisticHelper.Repository.IRepository;
using LogisticHelper.Repository;
using Microsoft.AspNetCore.Mvc;
using ServiceReference1;
using System.IO.Compression;
using System.Xml;
using Quartz;
using Quartz.Impl;
using System.Linq;
using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using LogisticHelper.Classes;

namespace LogisticHelper.Controllers
{
    public class SimcController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;


        public TerytWs1Client connection()
        {
            ServiceReference1.TerytWs1Client client = new ServiceReference1.TerytWs1Client();
            /*  serviceteryt.TerytWs1Client client = new serviceteryt.TerytWs1Client();*/


            client.ClientCredentials.UserName.UserName = "Mariusz.Sobota";
            client.ClientCredentials.UserName.Password = "so6QT8ahG";
            client.OpenAsync().Wait();
            return client;
        }
        [HttpPost]
        public string AutoComplete(string input)
        {
            TerytWs1Client client = connection();

            //create a list of all Terc elements
            IEnumerable<Simc> objSimcList = _unitOfWork.Simc.GetAll();



            //scan them
          var search = (from Simc in objSimcList
                          where

                          
                            Simc.NAZWA.StartsWith(input, StringComparison.InvariantCultureIgnoreCase)
                          orderby Simc.NAZWA.IndexOf(input),

                                   Simc.NAZWA.Length ascending
                          select new
                          {

                              label = Simc.NAZWA,
                              val = Simc.NAZWA,
                              woj = Simc.WOJ,
                              pow = Simc.POW,
                              gmi = Simc.GMI,
                              rodz_gmi = Simc.RODZ_GMI,
                              rm = Simc.RM,
                              mz = Simc.MZ,
                              nazwa = Simc.NAZWA,

                              sym = Simc.SYM,
                              sympod = Simc.SYMPOD,
                              stan_na = Simc.STAN_NA,



                          }).Take(5).ToList();

           


            string jsson = JsonConvert.SerializeObject(search);
            return jsson;
        }


        public SimcController(IUnitOfWork unitOfWork)
        {

            _unitOfWork = unitOfWork;
        }

/*        public IActionResult Index()
        {
            IEnumerable<Simc> objSimcList = _unitOfWork.Simc.GetAll();
            return View(objSimcList);
        }*/

        public IActionResult Search()
        {




            return View();
        }
      
  
         



         
        public IActionResult Found()
        {
           
         
             return View();
        }

        [HttpPost]
       
        public async Task<ActionResult> FoundAsync(string search)
        {
            var client = connection();
            //Zastanowić się jak rozgryźć wyszukiwarkę, 2 autocomplete? Jedna ze stringiem dla użytkownika, jedna dla sprzętu?
            var ss = AutoComplete(search);
            dynamic jsoon = JsonConvert.DeserializeObject(ss);
            var villagesArrays = new List<Miejscowosc[]> { };

            foreach (var obj in jsoon)
            {
                string objNazwa = obj.nazwa;
                string objSym = obj.sym;
                //  jsoon.Add(obj);

                villagesArrays.Add(await client.WyszukajMiejscowoscAsync(objNazwa, objSym)); // <---- Za każdym razem, tworzy się tutaj obiekt Miejscowość, teraz trzeba ją wyrucić na ekran

            }
            //WORKS!!!!!
            //Now have to write correct instruction to show data, but the principal of it works 
            //Whole JSON is being send, so np to choose data

            /*  foreach (ServiceReference1.Miejscowosc[] obj in villagesArrays)
              {
              }*/


            //Working!!!
            //Now find out how to show links on page!
          
            List<Miejscowosc> villages = new List<Miejscowosc>();

            foreach (var item in villagesArrays)
            {
                for (int i = 0; i < item.Length; i++)
                {
                    villages.Add(item[i]);
                    var powiat = item[i].Powiat;
                 

                }
            }

            

            return View(villages);
        }

        //Why symbol == null?
        //GET /Details/sym


        public async Task<IActionResult> DetailsAsync(string symbol, string wojewodztwo, string powiat, string nazwaMiejscowosci)
        {
            TerytWs1Client client = connection();

            if (symbol == null)
            {
                return NotFound();
            }
            List<Miejscowosc[]> administrativeUnits = new List<Miejscowosc[]>();
            administrativeUnits.Add(await client.WyszukajMiejscowoscAsync(nazwaMiejscowosci, symbol)); // <---- Za każdym razem, tworzy się tutaj obiekt Miejscowość, teraz trzeba ją wyrucić na ekran




            Simc getCityToSend = _unitOfWork.Simc.GetFirstOrDefault(u => u.SYM == symbol);
            List<UlicaDrzewo[]> city = new List<UlicaDrzewo[]>();
            //city.Add(getCityToSend);
            if (getCityToSend == null)
            {
                return NotFound();
            }


         

            //Sending data about Administrative Unit, to show to human
            TempData["wojewodztwo"] = wojewodztwo;
            TempData["powiat"] = powiat;
            TempData["nazwa"] = nazwaMiejscowosci;

            //Sending temp data to get streets
            TempData["WOJ"] = getCityToSend.WOJ;
            TempData["POW"] = getCityToSend.POW;
            TempData["GMI"] = getCityToSend.GMI;
            TempData["RODZ"] = getCityToSend.RODZ_GMI;
            TempData["SYM"] = getCityToSend.SYM;
      
          


            //city.Add(await client.PobierzListeUlicDlaMiejscowosciAsync(, , , getCityToSend.RODZ_GMI, getCityToSend.SYM, true, false, DateTime.Now));
            List<UlicaDrzewo> streets = new List<UlicaDrzewo>();
            foreach (var st in city)
            {
                for (int i = 0; i < st.Length; i++)
                {
                    streets.Add(st[i]);

                }

            }

            
          
            return RedirectToAction ("Index", "Ulic", streets);
                

        }
        public IActionResult City()
        {


            return View();
        }


        Simc addJA(string xWoj, string xPow, string xGmi, string xRodz,string xRm, string xNz, string xNazwa, string xSym, string xSympod, string xStan)
        {
            var additionQuery = new Simc
            {
                WOJ = xWoj,
                POW = xPow,
                GMI = xGmi,
                RODZ_GMI = xRodz,
                RM = xRm,
                MZ = xNz,
                NAZWA = xNazwa,
                SYM = xSym,
                SYMPOD = xSympod,
                STAN_NA = xStan
            };
            _unitOfWork.Simc.Add(additionQuery);
            _unitOfWork.Save();
            return additionQuery;
        }
        string handleNull(string xAfter, string xBefore)
        {
            if (xAfter == "")
                xAfter = xBefore;
            return xAfter;

        }


        public void Schedule()
        {
            TerytWs1Client client = connection();



            IEnumerable<Simc> SimcObjList = _unitOfWork.Simc.GetAll();


            //Teec is updated once a year

            DateTime prsD = DateTime.Now;
            string presentDate = prsD.ToShortDateString();

            DateTime pstD = prsD.AddYears(-1);
            string pastDate = pstD.ToShortDateString();


            



            //Here we have XML compressed to ZIP, now figure out how to suck it to db
            //FileChange is a variable in which is file, it doesnt exist phyisically on disc, how to unzip it?
            var UpdateFile = client.PobierzZmianySimcUrzedowyAsync(pstD, prsD);
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

            doc.Load(Directory.GetCurrentDirectory() + "/File/SIMC_Urzedowy_zmiany_" + pastDate + "_" + presentDate + ".xml");
            var xList = doc.SelectNodes("/zmiany/zmiana"); // Znajdź węzeł zmiany, w której znajdują się informacje dot. modernizacji
            foreach (XmlNode xNode in xList)
            {
                var xTypKorekty = xNode.SelectSingleNode("TypKorekty");
                switch (xTypKorekty.InnerText)
                {


                    //Dodanie jednostki administracyjnej
                    case "D": //**********************************************ADD ALL IMPORTANT SIMC STUFFF**********************************************
                        string xSym = (xNode.SelectSingleNode("Identyfikator").InnerText);
                        string xWojPo = (xNode.SelectSingleNode("WojPo").InnerText);
                        string xPowPo = (xNode.SelectSingleNode("PowPo").InnerText);
                        string xGmiPo = (xNode.SelectSingleNode("GmiPo").InnerText);
                        string xRodzPo = (xNode.SelectSingleNode("RodzPo").InnerText);
                        string xNazwaPo = xNode.SelectSingleNode("NazwaPo").InnerText;

                        string xRmPo = (xNode.SelectSingleNode("RodzajMiejscowosciPo").InnerText);
                        string xCzyNazwaZwyczajowaPo = (xNode.SelectSingleNode("CzyNazwaZwyczajowaPo").InnerText);
                        string xSympodPo = xNode.SelectSingleNode("IdentyfikatorMiejscowosciPodstawowejPo").InnerText;
                        string xStanPo = (xNode.SelectSingleNode("StanPo").InnerText);


                        var queryCheck = (from toj in SimcObjList where toj.SYM == xSym select toj).FirstOrDefault();

                        if (queryCheck != null)
                        {
                            break;
                        }
                        else
                        {

                            var queryAdd = addJA(xWojPo, xPowPo, xGmiPo, xRodzPo, xRmPo, xCzyNazwaZwyczajowaPo, xNazwaPo, xSym, xSympodPo, xStanPo);
                            // string xWoj, string xPow, string xGmi, string xRodz,string xRm, string xNz, string xNazwa, string xSym, string xSympod, string xStan
                            break;

                        }




                    //Delete old Jednostka Administracyjna
                    case "U":

                        string xSymPo = xNode.SelectSingleNode("Identyfikator").InnerText;



                        var query = (from toj in SimcObjList where toj.SYM == xSymPo select toj).FirstOrDefault();

                        //check if value was not changed before
                        if (query == null)
                            break;

                        _unitOfWork.Simc.Remove(query);
                        _unitOfWork.Save();

                        break;

                    default:
                        break;


                    //Update current Jednostka Administracyjne
                    //Modificate this method to work as in TERC, but for SIMC
                    case "Z":

                         //After change nodes
                        string xIdentyfikator = (xNode.SelectSingleNode("Identyfikator").InnerText);

                        string xWojPrzed = (xNode.SelectSingleNode("WojPrzed ").InnerText);
                        string xPowPrzed = (xNode.SelectSingleNode("PowPrzed ").InnerText);
                        string xGmiPrzed = (xNode.SelectSingleNode("GmiPrzed ").InnerText);
                        string xRodzPrzed = (xNode.SelectSingleNode("RodzPrzed ").InnerText);
                        string xNazwaPrzed = xNode.SelectSingleNode("NazwaPrzed ").InnerText;

                        string xRmPrzed = (xNode.SelectSingleNode("RodzajMiejscowosciPrzed ").InnerText);
                        string xCzyNazwaZwyczajowaPrzed = (xNode.SelectSingleNode("CzyNazwaZwyczajowaPrzed ").InnerText);
                        string xSympodPrzed = xNode.SelectSingleNode("IdentyfikatorMiejscowosciPodstawowejPrzed ").InnerText;
                        string xStanPrzed = (xNode.SelectSingleNode("StanPrzed ").InnerText);





                        xWojPo = handleNull((xNode.SelectSingleNode("WojPo").InnerText), xWojPrzed);
                        xPowPo = handleNull((xNode.SelectSingleNode("PowPo").InnerText), xPowPrzed);
                        xGmiPo = handleNull((xNode.SelectSingleNode("GmiPo").InnerText), xGmiPrzed);
                        xRodzPo = handleNull((xNode.SelectSingleNode("RodzPo").InnerText), xRodzPrzed);
                        xNazwaPo = handleNull(xNode.SelectSingleNode("NazwaPo").InnerText, xNazwaPrzed);

                        xRmPo = handleNull((xNode.SelectSingleNode("RodzajMiejscowosciPo").InnerText), xRmPrzed);
                        xCzyNazwaZwyczajowaPo = handleNull((xNode.SelectSingleNode("CzyNazwaZwyczajowaPo").InnerText), xCzyNazwaZwyczajowaPrzed);
                        xSympodPo = handleNull(xNode.SelectSingleNode("IdentyfikatorMiejscowosciPodstawowejPo").InnerText, xSympodPrzed);
                        xStanPo = handleNull((xNode.SelectSingleNode("StanPo").InnerText), xStanPrzed);




                        // etc

                        // Jak zrobić żeby ignorował wartości NULL ??



                        var queryZ = (from toj in SimcObjList where toj.SYM == xIdentyfikator select toj).FirstOrDefault();

                        //check if value was not changed before
                        if (queryZ == null)
                            break;
                        else
                        {
                            queryZ.WOJ = xWojPo;
                            queryZ.POW = xPowPo;
                            queryZ.GMI = xGmiPo;
                            queryZ.RODZ_GMI = xRodzPo;
                            queryZ.RM = xRmPo;
                            queryZ.MZ = xCzyNazwaZwyczajowaPo;
                            queryZ.NAZWA = xNazwaPo;
                            queryZ.SYM = xIdentyfikator;
                            queryZ.SYMPOD = xSympodPo;
                            queryZ.STAN_NA = xStanPo;
                        }




                        _unitOfWork.Simc.Update(queryZ);


                        _unitOfWork.Save();
                        break;


                    //WORKS, now polishing this little boy!!!


                    case "P":

                        //After change nodes
                         xIdentyfikator = (xNode.SelectSingleNode("Identyfikator").InnerText);

                         xWojPrzed = (xNode.SelectSingleNode("WojPrzed ").InnerText);
                         xPowPrzed = (xNode.SelectSingleNode("PowPrzed ").InnerText);
                         xGmiPrzed = (xNode.SelectSingleNode("GmiPrzed ").InnerText);
                         xRodzPrzed = (xNode.SelectSingleNode("RodzPrzed ").InnerText);
                         xNazwaPrzed = xNode.SelectSingleNode("NazwaPrzed ").InnerText;

                      
                         xStanPrzed = (xNode.SelectSingleNode("StanPrzed ").InnerText);





                        xWojPo = handleNull((xNode.SelectSingleNode("WojPo").InnerText), xWojPrzed);
                        xPowPo = handleNull((xNode.SelectSingleNode("PowPo").InnerText), xPowPrzed);
                        xGmiPo = handleNull((xNode.SelectSingleNode("GmiPo").InnerText), xGmiPrzed);
                        xRodzPo = handleNull((xNode.SelectSingleNode("RodzPo").InnerText), xRodzPrzed);
                        xNazwaPo = handleNull(xNode.SelectSingleNode("NazwaPo").InnerText, xNazwaPrzed);

                       
                        xStanPo = handleNull((xNode.SelectSingleNode("StanPo").InnerText), xStanPrzed);




                        // etc

                        // Jak zrobić żeby ignorował wartości NULL ??



                        var queryP = (from toj in SimcObjList where toj.SYM == xIdentyfikator select toj).FirstOrDefault();

                        //check if value was not changed before
                        if (queryP == null)
                            break;
                        else
                        {
                            queryP.WOJ = xWojPo;
                            queryP.POW = xPowPo;
                            queryP.GMI = xGmiPo;
                            queryP.RODZ_GMI = xRodzPo;

                            queryP.NAZWA = xNazwaPo;
                            queryP.SYM = xIdentyfikator;
                            
                        }




                        _unitOfWork.Simc.Update(queryP);


                        _unitOfWork.Save();
                        break;
                }

                //WORKS, now polishing this little boy!!!

            }
            // wezły


        }





        }



    }
