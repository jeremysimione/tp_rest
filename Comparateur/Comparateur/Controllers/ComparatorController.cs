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

namespace Comparateur.Controllers
{
    public class ComparatorController : ApiController
    {
        static List<HttpClient> listAgence = null;
        ObjectCache cache = MemoryCache.Default;
        CacheItemPolicy policy = new CacheItemPolicy();
        ComparatorController()
        {
            List<string> filePaths = new List<string>();
            filePaths.Add("c:\\cache\\example.txt");
            policy.ChangeMonitors.Add(new
            HostFileChangeMonitor(filePaths));
            if (cache["listagence"] == null)
            {
                listAgence = new List<HttpClient>();
                addAgence("https://localhost:44309/");
                addAgence("https://localhost:44310/");




            }
            else
            {
                listAgence = (List<HttpClient>)cache["listagence"];
            }


        }


        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
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
        [Route("comparateur/{ville}-{b:int}-{c:int}-{d:int}-{e:int}-{f:int}-{etoile:int}")]
        public Task<List<String>> Getoffre(string ville,int b, int c, int d, int e, int f,int etoile)
        {
            return GetAllOffers(ville,b,c,d,e,f,etoile);
        }

        [Route("agence")]
        public Task<List<String>> GetAgence1()
        {
            return GetAgence();
        }


        public void addAgence(String uri)
        {
            HttpClient c = new HttpClient();
            c.BaseAddress = new Uri(uri);
            listAgence.Add(c);
        }

        static async Task<List<string>> GetAgence()
        {
            int i = 0;
            List<String> s = new List<String>();
            foreach (HttpClient c in listAgence)
            {

                HttpResponseMessage response = await c.GetAsync("id");
                if (response.IsSuccessStatusCode)
                {
                    s.Add(i + " : " + await response.Content.ReadAsAsync<String>());
                }
                i++;
            }

            return s;
        }


        static async Task<List<string>> GetAllOffers(string ville, int dateArrivee, int datedepart, int prixmin, int prixmax, int nbDePersonnes,  int etoiles)
        {
            List<String> resu = new List<string>();
                List<int> s = new List<int>();
                string id;
                List<String> s1 = new List<String>();
            foreach (HttpClient c in listAgence)
            {
                HttpResponseMessage response1 = await c.GetAsync("id");
                if (response1.IsSuccessStatusCode)
                {
                    id = await response1.Content.ReadAsAsync<String>();
                    resu.Add("Les offres pour l'agence " + id + ": " );
                }

                HttpResponseMessage response = await c.GetAsync("rechercher/id/" + etoiles + "-" + ville);
                if (response.IsSuccessStatusCode)
                {
                    
                    s = await response.Content.ReadAsAsync<List<int>>();
                  
                }   

                for (int i = 0; i  <  s.Count; i++)
                {//hotel/rechercher/20210429-20210502-0-50000-5-0
                    HttpResponseMessage respons = await c.GetAsync("hotel/rechercher/" + dateArrivee + "-" + datedepart + "-" + prixmin + "-" + prixmax + "-" + nbDePersonnes + "-" +s.ElementAt(i));
                    if (response.IsSuccessStatusCode)
                    {
                        s1 = await respons.Content.ReadAsAsync<List<String>>();
                        foreach(string str in s1)
                        {
                            resu.Add(str);
                        }
                      
                    }

                }
                
            }

            return resu;

        }
    }
}

