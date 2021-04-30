using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Web.Http;

namespace Agence1.Controllers
{
    public class AgenceController : ApiController
    {
        static List<HttpClient> Hotel1 =null;
        ObjectCache cache = MemoryCache.Default;
        CacheItemPolicy policy = new CacheItemPolicy();
        public string id = "Agence1";
        private string mdp = "123456";




        AgenceController()
        {
            List<string> filePaths = new List<string>();
            filePaths.Add("c:\\cache\\example.txt");
            policy.ChangeMonitors.Add(new
            HostFileChangeMonitor(filePaths));
            if (cache["listhotel"] == null)
            {
                Hotel1 = new List<HttpClient>();
                    addHotel("https://localhost:44377/");
                    addHotel("https://localhost:44378/");


               
            }
            else
            {
                Hotel1 = (List<HttpClient>)cache["listhotel"];
            }


        }
        [Route("id")]
        public String GetAgence()
        {
            return id;
        }
        [Route("hotels")]
        public Task<List<Hotel>> GetHotels()
        {
          
            return GetHotel();
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
     
        [Route("hotel/rechercher/{b:int}-{c:int}-{d:int}-{e:int}-{f:int}-{identifiant:int}")]
        public Task<List<String>> Getrecherche(int b, int c, int d, int e, int f,int identifiant)
        { 
            return getPath(identifiant, "hotel/rechercher/"+b+"-"+c+"-"+d+"-"+e+"-"+f+"-"+id+"-"+mdp);
        }
        [Route("hotel/res/{b:int}-{nom}-{prenom}-{id:int}")]
        public Task<string> GetConfirm(int b,string nom,string prenom,int id)
        {
            return getPathString(id, "hotel/res/" + b + "-" + nom + "-" + prenom);
        }
        [Route("rechercher/{b:int}-{ville}")]
        public Task<List<String>> Getrec(int b, string ville)
        {
            return Getrecherchehotel(b, ville);
        }
        [Route("rechercher/id/{b:int}-{ville}")]
        public Task<List<int>> GetrecID(int b, string ville)
        {
            return GetRechercheIdHotel(b, ville);
        }
        [Route("image/{id:int}-{b:int}")]
        public Task<byte[]> Getim(int id, int b)
        {
            return getimage(id,b);
        }




        static async Task<List<String>> getPath(int id,string path)
        {
         
            List<String> s = null;
            HttpResponseMessage response = await Hotel1[id].GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                s = await response.Content.ReadAsAsync<List<String>>();
            }
            
            return s;
        }
        static async Task<byte[]> getimage(int id, int b)
        {

            byte[] s = null;
            HttpResponseMessage response = await Hotel1[id].GetAsync("hotel/image/"+b);
            if (response.IsSuccessStatusCode)
            {
                s = await response.Content.ReadAsAsync<byte[]>();
            }

            return s;
        }
        static async Task<String> getPathString(int id, string path)
        {

            String s = null;
            HttpResponseMessage response = await Hotel1[id].GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                s = await response.Content.ReadAsAsync<String>();
            }

            return s;
        }

        public void addHotel(String uri)
        {
            HttpClient c = new HttpClient();
            c.BaseAddress = new Uri(uri);
            Hotel1.Add(c);
        }

        static async Task<List<Hotel>> GetHotel()
        {
            List<Hotel> s = new List<Hotel>();
            foreach (HttpClient c in Hotel1) 
            {

                HttpResponseMessage response = await c.GetAsync("hotel/2");
                if (response.IsSuccessStatusCode)
                {
                    s.Add(await response.Content.ReadAsAsync<Hotel>());
                }
            }

            return s;
        }
        static async Task<List<string>> Getrecherchehotel(int etoiles,string ville)
        {
            List<String> resu = new List<string>();
            List<Hotel> s = new List<Hotel>();
            foreach (HttpClient c in Hotel1)
            {

                HttpResponseMessage response = await c.GetAsync("hotel/2");
                if (response.IsSuccessStatusCode)
                {
                    s.Add(await response.Content.ReadAsAsync<Hotel>());
                }
            }
            for(int i = 0; i < s.Count; i++)
            {
               if( s[i].lieu==ville && s[i].nbEtoiles == etoiles)
                {
                    resu.Add(i+ ": "+  s[i].nom+" "+s[i].adresse);

                }
            }

            return resu;
        }
        static async Task<List<int>> GetRechercheIdHotel(int etoiles, string ville)
        {
            List<int> resu = new List<int>();
            List<Hotel> s = new List<Hotel>();
            foreach (HttpClient c in Hotel1)
            {

                HttpResponseMessage response = await c.GetAsync("hotel/2");
                if (response.IsSuccessStatusCode)
                {
                    s.Add(await response.Content.ReadAsAsync<Hotel>());
                }
            }
            for (int i = 0; i < s.Count; i++)
            {
                if (s[i].lieu == ville && s[i].nbEtoiles == etoiles)
                {
                    resu.Add(i);

                }
            }

            return resu;
        }


    }
    public class Hotel
    {
        [DataMember]
        public int nbDeChambres;
        [DataMember]
        public String lieu;
        [DataMember]
        public String adresse;
        [DataMember]
        public String nom;
        [DataMember]
        public int nbEtoiles;
        
    }
    }
