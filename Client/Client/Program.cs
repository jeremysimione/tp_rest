using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;

namespace Client
{
    class interfaceClient

    {
        private static Size GetConsoleFontSize()
        {
            // getting the console out buffer handle
            IntPtr outHandle = CreateFile("CONOUT$", GENERIC_READ | GENERIC_WRITE,
                FILE_SHARE_READ | FILE_SHARE_WRITE,
                IntPtr.Zero,
                OPEN_EXISTING,
                0,
                IntPtr.Zero);
            int errorCode = Marshal.GetLastWin32Error();
            if (outHandle.ToInt32() == INVALID_HANDLE_VALUE)
            {
                throw new IOException("Unable to open CONOUT$", errorCode);
            }

            ConsoleFontInfo cfi = new ConsoleFontInfo();
            if (!GetCurrentConsoleFont(outHandle, false, cfi))
            {
                throw new InvalidOperationException("Unable to get font information.");
            }

            return new Size(cfi.dwFontSize.X, cfi.dwFontSize.Y);
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateFile(
            string lpFileName,
            int dwDesiredAccess,
            int dwShareMode,
            IntPtr lpSecurityAttributes,
            int dwCreationDisposition,
            int dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetCurrentConsoleFont(
            IntPtr hConsoleOutput,
            bool bMaximumWindow,
            [Out][MarshalAs(UnmanagedType.LPStruct)] ConsoleFontInfo lpConsoleCurrentFont);

        [StructLayout(LayoutKind.Sequential)]
        internal class ConsoleFontInfo
        {
            internal int nFont;
            internal Coord dwFontSize;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct Coord
        {
            [FieldOffset(0)]
            internal short X;
            [FieldOffset(2)]
            internal short Y;
        }

        private const int GENERIC_READ = unchecked((int)0x80000000);
        private const int GENERIC_WRITE = 0x40000000;
        private const int FILE_SHARE_READ = 1;
        private const int FILE_SHARE_WRITE = 2;
        private const int INVALID_HANDLE_VALUE = -1;
        private const int OPEN_EXISTING = 3;

        static HttpClient client = new HttpClient();
        static HttpClient comparateur = new HttpClient();



        public interfaceClient()
        {
            client.BaseAddress = new Uri("https://localhost:44309/");
            comparateur.BaseAddress = new Uri("https://localhost:44390");
        }
        static async Task<List<String>> getrecherchehotel(int etoile, string ville)
        {


            List<string> s = null;
            HttpResponseMessage response = await client.GetAsync("rechercher/" + etoile + "-" + ville);
            if (response.IsSuccessStatusCode)
            {
                s = await response.Content.ReadAsAsync<List<string>>();
            }

            return s;
        }
        static async Task<List<String>> getrechercheres(int b, int c, int d, int e, int f, int identifiant)
        {



            List<string> s = null;
            HttpResponseMessage response = await client.GetAsync("hotel/rechercher/" + b + "-" + c + "-" + d + "-" + e + "-" + f + "-" + identifiant);
            if (response.IsSuccessStatusCode)
            {
                s = await response.Content.ReadAsAsync<List<string>>();
            }

            return s;

        }

        static async Task<String> getConfirmation(int numres, string nom, string prenom, int id)
        {
            string s = null;
            HttpResponseMessage response = await client.GetAsync("hotel/res/" + numres + "-" + nom + "-" + prenom + "-" + id);
            if (response.IsSuccessStatusCode)
            {
                s = await response.Content.ReadAsAsync<string>();
            }

            return s;

        }
        static async Task<byte[]> getImage(int id, int b)
        {
            byte[] s = null;
            HttpResponseMessage response = await client.GetAsync("image/" + id + "-" + b);
            if (response.IsSuccessStatusCode)
            {
                s = await response.Content.ReadAsAsync<byte[]>();
            }

            return s;

        }

        static async Task<List<String>> getAllOffers(string ville, int dateArrivee, int dateDepart, int prixMin, int prixMax, int nbDePersonnes, int nbEtoiles)
        {
            List<String> s = null;

            HttpResponseMessage response = await comparateur.GetAsync("comparateur/" + ville + "-" + dateArrivee + "-" + dateDepart + "-" + prixMin + "-" + prixMax + "-" + nbDePersonnes + "-" + nbEtoiles);
            if (response.IsSuccessStatusCode)
            {
                s = await response.Content.ReadAsAsync<List<String>>();
            }

            return s;

        }

        static async Task<String> getIDAgence()
        {
            String s = null;

            HttpResponseMessage response = await client.GetAsync("id");
            if (response.IsSuccessStatusCode)
            {
                s = await response.Content.ReadAsAsync<String>();
            }

            return s;

        }


        static void Main(string[] args)
        {
            interfaceClient ic = new interfaceClient();
            string ville;
            List<String> r;
            int stars;

            bool x = true;
            while (x)
            {
                Console.WriteLine("Vous êtes connecté sur l'agence :" + getIDAgence().Result);
                Console.WriteLine("Vous voulez lancer le comparateur avant de rechercher?");
                string comp = Console.ReadLine();
                if (comp == "oui" || comp == "yes" || comp == "o")
                {
                    Console.WriteLine("Quel est la ville souhaitée ? ");

                    ville = Console.ReadLine();
                    Console.WriteLine("Quel est le nb d'etoiles ?");

                    stars = int.Parse(Console.ReadLine());
                  
                        Console.WriteLine("Quel est le nombre de personnes ?");

                        int nbdepers = int.Parse(Console.ReadLine());


                        Console.WriteLine("Quel est la date d'arrivée (au format aaaammjj) ");

                        int dateArrivee = int.Parse(Console.ReadLine());

                        Console.WriteLine("Quel est la date de départ (au format aaaammjj) ");

                        int dateDepart = int.Parse(Console.ReadLine());

                        Console.WriteLine(dateDepart);

                        Console.WriteLine("Quel est le prix min ?");

                        int prixMin = int.Parse(Console.ReadLine());


                        Console.WriteLine("Quel est le prix max ?");

                        int prixMax = int.Parse(Console.ReadLine());

                        List<String> str = new List<String>();

                        str = getAllOffers(ville, dateArrivee, dateDepart, prixMin, prixMax, nbdepers, stars).Result;

                        Console.WriteLine("Voici la liste des offres : ");
                        foreach (string s10 in str)
                        {
                            Console.WriteLine(s10);
                        }
                    }
                    Console.WriteLine("Recherche d'hotel :");
                    Console.WriteLine("Pour effectuer une reservation sur l'agence : "+ getIDAgence().Result+" veuillez saisir les informations ci-dessous ");

                    Console.WriteLine("Quel est la ville souhaitée ? ");

                    ville = Console.ReadLine();
                    Console.WriteLine("Quel est le nb d'etoiles ? ");

                    stars = int.Parse(Console.ReadLine());


                    r = getrecherchehotel(stars, ville).Result;
                    if (r.Count != 0)
                    {
                        Console.WriteLine("Voici la liste des hôtels disponibles : ");
                        for (int i = 0; i < r.Count; i++)
                        {
                            Console.WriteLine(r.ElementAt(i));

                        }


                        Console.WriteLine("Quel est le nombre de personnes ? ");

                        int nbdepers = int.Parse(Console.ReadLine());


                        Console.WriteLine("Quel est la date d'arrivée (au format aaaammjj ");

                        int dateArrivee = int.Parse(Console.ReadLine());

                        Console.WriteLine("Quel est la date de départ (au format aaaammjj) ");

                        int dateDepart = int.Parse(Console.ReadLine());

                        Console.WriteLine(dateDepart);

                        Console.WriteLine("Quel est le prix min ? ");

                        int prixMin = int.Parse(Console.ReadLine());


                        Console.WriteLine("Quel est le prix max ? ");

                        int prixMax = int.Parse(Console.ReadLine());



                        Console.WriteLine("Quell est id de l'hotel que vous voulez ? ");
                        int idhotel = int.Parse(Console.ReadLine());
                        // Console.WriteLine("Identification :");
                        //   Console.WriteLine("saisissez votre identifiant :");
                        // String id1 = Console.ReadLine();
                        //Console.WriteLine("saisissez votre Mot de passe");
                        // String MDP1 = Console.ReadLine();
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine();
                        Console.WriteLine();
                        List<String> r1 = getrechercheres(dateArrivee, dateDepart, prixMin, prixMax, nbdepers, idhotel).Result;
                        if (r1.Count > 0)
                        {
                            for (int i = 0; i < r1.Count; i++)
                                Console.WriteLine(r1.ElementAt(i));
                            Size imageSize = new Size(20, 10); // desired image size in characters

                            // draw some placeholders


                            using (Graphics g = Graphics.FromHwnd(GetConsoleWindow()))
                            {
                                using (Image image = (Bitmap)((new ImageConverter()).ConvertFrom(getImage(idhotel, 0).Result)))
                                {
                                    Size fontSize = GetConsoleFontSize();

                                    // translating the character positions to pixels
                                    Rectangle imageRect = new Rectangle(
                                        0 * fontSize.Width,
                                       16 * fontSize.Height,
                                        imageSize.Width * fontSize.Width,
                                        imageSize.Height * fontSize.Height);
                                    g.DrawImage(image, imageRect);
                                }
                            }

                            using (Graphics g = Graphics.FromHwnd(GetConsoleWindow()))
                            {
                                using (Image image = (Bitmap)((new ImageConverter()).ConvertFrom(getImage(idhotel, 12).Result)))
                                {
                                    Size fontSize = GetConsoleFontSize();

                                    // translating the character positions to pixels
                                    Rectangle imageRect = new Rectangle(
                                       30 * fontSize.Width,
                                       16 * fontSize.Height,
                                        imageSize.Width * fontSize.Width,
                                        imageSize.Height * fontSize.Height);
                                    g.DrawImage(image, imageRect);
                                }
                            }
                            Console.WriteLine("Si vous voulez confirmer votre reservation tapez l'identifiant Unique de votre offre :");
                            int idoffre = int.Parse(Console.ReadLine());
                            Console.WriteLine("Saisissez le nom de la personne principale à	héberger :");
                            String nom = Console.ReadLine();
                            Console.WriteLine("Saisissez le prenom de la personne principale à héberger :");
                            String prenom = Console.ReadLine();

                            Console.WriteLine(getConfirmation(idoffre, nom, prenom, idhotel).Result);
                        }
                        else
                        {
                            Console.WriteLine("Aucune offre disponible");

                        }



                        Console.WriteLine("Voulez vous effectuer une nouvelle recherche ?");
                        String rep1 = Console.ReadLine();
                        if (rep1 == "non")
                        {
                            x = false;
                        }


                    }

                }

            }
        }

    }

