using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Caching;
using System.Runtime.Serialization;
using System.Web.Http;
using System.Xml.Serialization;


namespace TP_Rest.Controllers
{
    public class HotelController : ApiController
    {
        Hotel h;
        ObjectCache cache = MemoryCache.Default;
        CacheItemPolicy policy = new CacheItemPolicy();


        HotelController()
        {
            List<string> filePaths = new List<string>();
            filePaths.Add("c:\\cache\\example.txt");

            policy.ChangeMonitors.Add(new
            HostFileChangeMonitor(filePaths));
            if (cache["hotel"] == null)
            {
                List<IChambre> chambres = new List<IChambre>();
                List<Reservation> resas = new List<Reservation>();
                for (int i = 0; i < 10; i++)
                {
                    chambres.Add(new ChambreDouble(new Chambre("chambredouble2.jpg", 1, 45, i)));
                }

                Console.WriteLine(chambres.ElementAt(0).getPrixParNuitTTC());

                for (int i = 0; i < 10; i++)
                {
                    chambres.Add(new ChambreSimple(new Chambre("chambres2.jpg", 1, 45, i)));

                }
                h = new Hotel(4, "montpellier", "31 rue de la loge", "Companile", 5, chambres);

                cache.Set("hotel", h, policy);
            }
            else
            {
                h = (Hotel)cache["hotel"];
            }

            h.AddAgence("Agence1",  "123456",  0.25);
            h.AddAgence("Agence2", "123456", 0.10);
      

        }
        // GET api/values
        public IEnumerable<string> Get()
        {
           
            return new string[] { "value1", "value2" };
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
        [Route("hotel/{b:int}")]
        public Hotel Gethotel(int b)
        {
            
            return h;
        }
        [Route("hotel/rechercher/{b:int}-{c:int}-{d:int}-{e:int}-{f:int}-{id}-{mdp}")]
        
            public List<string> Getrecherche(int b, int c, int d, int e, int f ,string id,string mdp)
        {
            int d1 = b % 100;
            int m = (b / 100) % 100;
            int y = b / 10000;
            int d2 = c % 100;
            int m1 = (c / 100) % 100;
            int y1 = c / 10000;

            return  h.rechercher(new DateTime(y, m, d1), new DateTime(y1, m1, d2), d, e, f, id,mdp);
        }//
        [Route("hotel/res/{b:int}")]
        public String Getres(int b)
        {
            return h.reserFinal(b, DateTime.Today, DateTime.Today.AddDays(4), "aiss", "aiss");
        }
        [Route("hotel/image/{b:int}")]
        public byte[] GetImageFile(int b)
        {
            if (System.IO.File.Exists(System.Web.Hosting.HostingEnvironment.MapPath("~/TransientStorage/") +h.chambres.ElementAt(b).getImagePath()))
                return System.IO.File.ReadAllBytes(System.Web.Hosting.HostingEnvironment.MapPath("~/TransientStorage/") + h.chambres.ElementAt(b).getImagePath());
            else
                return new byte[]{0};
        }





    }
    public class Reservation
    {
        String nom;
        String Prenom;
        DateTime startDate;
        DateTime endDate;
        List<IChambre> c;
        Hotel h;
        int nbdeJours;
        static List<Agence> agences = new List<Agence>();
        public Reservation()
        {

        }
        public Reservation(DateTime startDate, DateTime endDate, List<IChambre> c, Hotel h)
        {
            this.startDate = startDate;
            this.endDate = endDate;
            this.c = c;
            this.h = h;
        }
        public Reservation(DateTime startDate, DateTime endDate, Hotel h, String nom, String prenom)
        {
            this.startDate = startDate;
            this.endDate = endDate;
            this.h = h;
            this.nom = nom;
            this.Prenom = prenom;

        }
       
        public DateTime getStartDate()
        {
            return this.startDate;
        }
        public DateTime getEndDate()
        {
            return this.endDate;
        }
        public Double getLongueurResa()
        {
            return (endDate.Date - startDate.Date).TotalDays;
        }




    }
    public class Agence
    {
        private String identifiant;
        private String motDePasse;
        public double taux;
        public Agence()
        {

        }
        public Agence(String a, String b,double c)
        {
            this.identifiant = a;
            this.motDePasse = b;
            this.taux= c;

        }
        public string getID()
        {
            return this.identifiant;
        }
        public string getMDP()
        {
            return this.motDePasse;
        }

        public Double getTaux()
        {
            return this.taux;
        }
    }
    [DataContract]
    [KnownType(typeof(ChambreDouble))]
    [KnownType(typeof(ChambreSimple))]
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
        public List<IChambre> chambres = new List<IChambre>();
        public Dictionary<int,List<int>> dico = new Dictionary<int, List<int>>();
        public List<Agence> agence = new List<Agence>();
        public Hotel()
        {

        }
        public Hotel(int nbDeChambres, String lieu, String adresse, String nom, int nbEtoiles)
        {
            this.nbDeChambres = nbDeChambres;
            this.lieu = lieu;
            this.adresse = adresse;
            this.nom = nom;
            this.nbEtoiles = nbEtoiles;
        }
        public Hotel(int nbDeChambres, String lieu, String adresse, String nom, int nbEtoiles, List<IChambre> c)
        {
            this.nbDeChambres = nbDeChambres;
            this.lieu = lieu;
            this.nom = nom;
            this.adresse = adresse;
            this.nbEtoiles = nbEtoiles;
            this.chambres = c;
        }


        public String getNom()
        {
            return nom;
        }
        public void AddAgence(string id,string mdp,double x)
        {

            agence.Add(new Agence(id,mdp,x));
          }
        public List<IChambre> getChambres()
        {
            return this.chambres;
        }
        public int getNbdeChambres()
        {
            return chambres.Count;
        }
        public String setnom()
        {
            this.nom = "test";
            return "test";
        }

        public List<int> reserver(DateTime arrivee, DateTime depart, int nbDePersonnes)
        {
            int nbChambreDouble = nbDePersonnes / 2;
            int nbChambreSimple = nbDePersonnes % 2;
            int x = 0; //nb de chambre dispo
            int y = 0;
            List<int> cham = new List<int>();

            for (int i = 0; i < chambres.Count; i++)
            {
                if (chambres.ElementAt(i).getNbDePersonnes() == 2)
                {
                    if (chambres.ElementAt(i).chambreLibre(arrivee, depart) && x < nbChambreDouble)
                    {

                        x += 1;
                        cham.Add(i);

                    }
                }

                if (chambres.ElementAt(i).getNbDePersonnes() == 1)
                {
                    if (chambres.ElementAt(i).chambreLibre(arrivee, depart) && x < nbChambreSimple)
                    {

                        y += 1;
                        cham.Add(i);

                    }
                }



            }
            return cham;
        }
        public List<int> reserverSimple(DateTime arrivee, DateTime depart, int nbDePersonnes)
        {

            int nbChambreSimple = nbDePersonnes;
            int x = 0; //nb de chambre dispo
            List<int> cham = new List<int>();

            for (int i = 0; i < chambres.Count; i++)
            {


                if (chambres.ElementAt(i).getNbDePersonnes() == 1)
                {
                    if (chambres.ElementAt(i).chambreLibre(arrivee, depart) && x < nbChambreSimple)
                    {

                        x += 1;
                        cham.Add(i);

                    }
                }



            }
            return cham;
        }
        public void confirmerRes(List<int> c, DateTime arrivee, DateTime depart, String nom, String prenom)
        {
            foreach (int ch in c)
            {
                chambres.ElementAt(ch).addReservation(new Reservation(arrivee, depart, this, nom, prenom));
            }
        }

        public Boolean resrLibreSimple(int nbdePersonnes, DateTime startDate, DateTime endDate)
        {
            int y = 0;
            foreach (IChambre ch in chambres)
            {
                if (ch.getNbDePersonnes() == 1)
                {
                    if (ch.chambreLibre(startDate, endDate))
                    {
                        y += 1;

                    }

                }

            }
            return y >= nbdePersonnes;
        }

        public Boolean resrLibreDouble(int nbdePersonnes, DateTime startDate, DateTime endDate)
        {
            int nbChambreDouble = nbdePersonnes / 2;
            int nbChambreSimple = nbdePersonnes % 2;
            int x = 0; //nb de chambre dispo
            int y = 0;
            foreach (IChambre ch in chambres)
            {
                if (ch.getNbDePersonnes() == 2)
                {
                    if (ch.chambreLibre(startDate, endDate))
                    {
                        x += 1;

                    }

                }
                if (ch.getNbDePersonnes() == 1)
                {
                    if (ch.chambreLibre(startDate, endDate))
                    {
                        y += 1;

                    }

                }

            }
            return x >= nbChambreDouble && y >= nbChambreSimple;
        }


        public Double getPrixSejourChambreDouble(int nbdePersonnes, DateTime startDate, DateTime endDate, Agence a)
        {

            int duree = (int)(endDate.Date - startDate.Date).TotalDays;
            int prixTTC = 0;

            int nbChambreDouble = nbdePersonnes / 2;
            int nbChambreSimple = nbdePersonnes % 2;
            for (int i = 0; i < nbChambreDouble; i++)
            {
                foreach (IChambre ch in chambres)
                {
                    if (ch.getNbDePersonnes() == 2)
                    {
                        prixTTC += ch.getPrixParNuitTTC();
                        break;
                    }

                }
            }
            for (int i = 0; i < nbChambreSimple; i++)
            {
                foreach (IChambre ch in chambres)
                {
                    if (ch.getNbDePersonnes() == 1)
                    {
                        prixTTC += ch.getPrixParNuitTTC();
                        break;
                    }
                }
            }
            return (prixTTC * duree) - (prixTTC * duree) * a.getTaux();
        }
        public Double getPrixSejourChambreSimple(int nbdePersonnes, DateTime startDate, DateTime endDate, Agence a)
        {

            int duree = (int)(endDate.Date - startDate.Date).TotalDays;
            int prixTTC = 0;


            int nbChambreSimple = nbdePersonnes;

            for (int i = 0; i < nbChambreSimple; i++)
            {
                foreach (IChambre ch in chambres)
                {
                    if (ch.getNbDePersonnes() == 1)
                    {
                        prixTTC += ch.getPrixParNuitTTC();
                        break;
                    }
                }
            }
            return (prixTTC * duree) - (prixTTC * duree) * a.getTaux();
        }

        public String getVille()
        {
            return this.lieu;
        }
        public int getNbEtoiles()
        {
            return this.nbEtoiles;
        }
        public List<String> rechercher(DateTime arrivee, DateTime depart, int prixMin, int prixMax, int nbDePersonnes, string id,string mdp)
        {

            List<String> resu = new List<String>();
            Agence a =null;
            foreach(Agence b in agence){
               if( b.getID() == id)
                {
                    if (b.getMDP() == mdp)
                    {
                        a = b;
                    }
                }
            }

                if (a != null)
                {
                    if (this.getPrixSejourChambreDouble(nbDePersonnes, arrivee, depart, a) < prixMax
                            && this.getPrixSejourChambreDouble(nbDePersonnes, arrivee, depart, a) > prixMin && (this.resrLibreDouble(nbDePersonnes, arrivee, depart) || this.resrLibreSimple(nbDePersonnes, arrivee, depart)))
                    {
                        if (this.resrLibreDouble(nbDePersonnes, arrivee, depart))
                        {
                            Random aleatoire = new Random();
                            int x = aleatoire.Next(1111111, 9999999);
                            while (dico.ContainsKey(x))
                            {
                                x = aleatoire.Next(1111111, 9999999);
                            }

                            resu.Add(x + " :" + this.getNom() + " au prix Ttc : " + this.getPrixSejourChambreDouble(nbDePersonnes, arrivee, depart, a) + "Euros (" + nbDePersonnes / 2 + "chambre double et " + nbDePersonnes % 2 + "Chambre Simple)");

                            dico.Add(x, this.reserver(arrivee, depart, nbDePersonnes));

                        }
                        if (this.resrLibreSimple(nbDePersonnes, arrivee, depart))
                        {
                            Random aleatoire = new Random();
                            int x = aleatoire.Next(1111111, 9999999);
                            while (dico.ContainsKey(x))
                            {
                                x = aleatoire.Next(1111111, 9999999);
                            }
                            dico.Add(x, this.reserverSimple(arrivee, depart, nbDePersonnes));

                            resu.Add(x + " :" + this.getNom() + " au prix Ttc : " + this.getPrixSejourChambreSimple(nbDePersonnes, arrivee, depart, a) + "Euros (" + nbDePersonnes + "Chambre Simple)");


                        }


                    }
            }
            else { resu.Add("Votre identifiant et/ou votre mot de passe n'est pas correct ! ");  }


            return resu;
        }
        public String reserFinal(int x, DateTime arr, DateTime fin, String nom, String prenom)
        {

            if (dico.ContainsKey(x))
            {
                this.confirmerRes(dico[x], arr, fin, nom, prenom);
                return "Votre réservation a été effectué avec succés";
            }
            return "Votre id est incorrect ";

        }

        


    }
    public abstract class IChambre
    {

        public abstract int getNbDePersonnes();

        public abstract int getNumero();
        public abstract Boolean chambreLibre(DateTime start, DateTime end);

        public abstract int getPrixParNuitTTC();
        public abstract List<Reservation> getResr();

        public abstract String getImagePath();
        public abstract void addReservation(Reservation r);


    }
    public class Chambre : IChambre
    {
        public int nbDePersonnes;
        public string imagePath;
        public int prixParNuitTTC;
        public int numero;
        public List<Reservation> reservations = new List<Reservation>();

        public Chambre()
        {

        }
        public Chambre(string imagePath, int nbDePersonnes, int prixParNuitTTC, int numero)
        {
            this.nbDePersonnes = nbDePersonnes;
            this.imagePath = imagePath;
            this.prixParNuitTTC = prixParNuitTTC;
            this.numero = numero;
        }
        public override Boolean chambreLibre(DateTime start, DateTime end)
        {

            Boolean b = true;
            if (reservations != null)
            {
                foreach (Reservation r in reservations)
                {
                    if (r.getStartDate() < start)
                    {

                        if (r.getEndDate() > start)
                        {
                            b = false;
                        }
                    }
                    else if (r.getStartDate() < end)
                    {
                        b = false;
                    }
                }
            }
            return b;
        }

        public override string getImagePath()
        {
            return this.imagePath;
        }

        public override int getNumero()
        {
            return this.numero;
        }

        public override int getPrixParNuitTTC()
        {
            return this.prixParNuitTTC;
        }

        public override int getNbDePersonnes()
        {
            return this.nbDePersonnes;
        }

        public override void addReservation(Reservation r)
        {
            reservations.Add(r);
        }

        public override List<Reservation> getResr()
        {
            return reservations;
        }
    }

    public class ChambreDecorateur : IChambre
    {
        private IChambre _decore;

        public ChambreDecorateur()
        {
        }

        public ChambreDecorateur(IChambre c)
        {
            this._decore = c;
        }

        public override void addReservation(Reservation r)
        {
            _decore.addReservation(r);
        }

        public override bool chambreLibre(DateTime start, DateTime end)
        {
            return _decore.chambreLibre(start, end);
        }

        public override string getImagePath()
        {
            return _decore.getImagePath();
        }

        public override int getNbDePersonnes()
        {
            return this._decore.getNbDePersonnes();

        }

        public override int getNumero()
        {
            return this._decore.getNumero();
        }

        public override int getPrixParNuitTTC()
        {
            return this._decore.getPrixParNuitTTC();
        }

        public override List<Reservation> getResr()
        {
            return _decore.getResr();
        }
    }


        public class ChambreSimple : ChambreDecorateur
        {
            public int nbDePersonnes;
            public int prixParNuitTTC;
            public ChambreSimple() : base()
            {

            }
            public ChambreSimple(IChambre c) : base(c)
            {
                this.nbDePersonnes = base.getNbDePersonnes();
                this.prixParNuitTTC = base.getPrixParNuitTTC();

            }

            public override int getNumero()
            {
                return base.getNumero();
            }

            public override int getPrixParNuitTTC()
            {
                return base.getPrixParNuitTTC();
            }

            public override int getNbDePersonnes()
            {
                return base.getNbDePersonnes();
            }


        }
        public class ChambreDouble : ChambreDecorateur
        {
            public int nbDePersonnes;
            public int prixParNuitTTC;

            public ChambreDouble() : base()
            {

            }
            public ChambreDouble(IChambre c) : base(c)
            {
                nbDePersonnes = base.getNbDePersonnes() * 2;
                prixParNuitTTC = base.getPrixParNuitTTC() * 2 - 20;
            }

            public override int getNumero()
            {
                return base.getNumero();
            }

            public override int getPrixParNuitTTC()
            {
                return this.prixParNuitTTC;
            }

            public override int getNbDePersonnes()
            {
                return this.nbDePersonnes;
            }

        }
}

