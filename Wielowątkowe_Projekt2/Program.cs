using System;
using System.Threading;
using System.Collections.Generic;
using static System.Console;
namespace Wielowątkowe_Projekt2
{
    // Opis
    // Założenia:
    // Przyjąłem k=5 jak w problemie, ale program by mógł obsłyżyć i inne ilości
    // Analogicznie przyjąłem taką samą liczbę miejsc, widelcy, książek i filozodów jak w problemie.
    /*
        Sposób działania; Żeby filozof mógł czytać książkę podczas jedzenia to znaczy, że pierw musi ją wybrać (zanim usiądzie lub zacznie jeść).
        W związku z tym pierw każdy z filozofów wybiera książki, następnie miejsce (bo by jeść trzeba pierw siedzieć). A na końcu jedzą.
        Każdy z filozofów jest osobnym wątkiem wewnątrz którego są 3 zamki dotyczące tych 3 sekcji (wyboru ksiązki,wyboru  miejsca, jedzenia).
    */

    class Program
    {
        class filozofowie
        {
            // do zamków
            Object Wybór_Ksiazki = new Object();
            Object Wybór_Miejsca = new Object();
            Object Wybór_sztućca = new Object();

            // W listach sa numery poszczegolnych miejsc, ksiazek, widelcy
            List<int> Ksiazki = new List<int>() { 1, 2, 3, 4, 5 };   // Czy ksiazka wolna? (na poczatku wszystkie są wolne)
            List<int> Miejsca = new List<int>() { 1, 2, 3, 4, 5 };   // Czy miejsce wolne? (na poczatku wszystkie są wolne)
            List<int> Widelce = new List<int>() { 1, 2, 3, 4, 5 };   // Czy widelec wolny? (na poczatku wszystkie są wolne)

            [ThreadStatic]
            static int threadSpecificData;

            public void Filozof()
            {
                threadSpecificData = Thread.CurrentThread.ManagedThreadId-4;

                lock (Wybór_Ksiazki)
                {
                    Random seed1 = new Random();
                    //wybór książki
                    int pozycja_ksiazki_w_zbiorze = seed1.Next(0, Ksiazki.Count); // Losuje indeks książki z listy
                    int ksiazka_wybrana = Ksiazki[pozycja_ksiazki_w_zbiorze]; // Wybiera książkę spod danego indeksu

                    //usuniecie ksiazki z listy
                    Ksiazki.RemoveAt(pozycja_ksiazki_w_zbiorze); //Usuwa książkę spod danego indeksu (bo jest zabrana)

                    WriteLine("Filozof "+ threadSpecificData +" wybrał książkę: " + (ksiazka_wybrana));
                }

                int miejsce_wybrane;
                lock (Wybór_Miejsca)
                {
                    Random seed2 = new Random();
                    //wybór miejsca
                    int pozycja_miejsca_w_zbiorze = seed2.Next(0, Miejsca.Count);
                    miejsce_wybrane = Miejsca[pozycja_miejsca_w_zbiorze];

                    //usuniecie miejsca z listy
                    Miejsca.RemoveAt(pozycja_miejsca_w_zbiorze);

                    WriteLine("Filozof " + threadSpecificData + " wybrał miejsce: " + (miejsce_wybrane));
                }
                    // Widelce
                    bool czy_głodny = true;
                    while (czy_głodny)
                    { 

                        lock (Wybór_sztućca)
                        { 
                            if (miejsce_wybrane == 1)
                            {
                                if (Widelce.Contains(5) && Widelce.Contains(1))
                                {
                                    Widelce.Remove(5);
                                    Widelce.Remove(1);

                                    WriteLine("Filozof " + threadSpecificData + " na miejscu: " + (miejsce_wybrane) + " zaczął jeść");

                                    Thread.Sleep(10);

                                    WriteLine("Filozof " + threadSpecificData + " na miejscu: " + (miejsce_wybrane) + " zjadł");

                                    Widelce.Add(5);
                                    Widelce.Add(1);

                                    czy_głodny = false;
                                }
                            }
                            else
                            {
                                if (Widelce.Contains(miejsce_wybrane - 1) && Widelce.Contains(miejsce_wybrane))
                                {
                                    Widelce.Remove(miejsce_wybrane-1);
                                    Widelce.Remove(miejsce_wybrane);

                                    WriteLine("Filozof " + threadSpecificData + " na miejscu: " + (miejsce_wybrane) + " zaczął jeść");

                                    Thread.Sleep(10);


                                    WriteLine("Filozof " + threadSpecificData + " na miejscu: " + (miejsce_wybrane) + " zjadł");

                                    Widelce.Add(miejsce_wybrane-1);
                                    Widelce.Add(miejsce_wybrane);

                                    czy_głodny = false;
                                }
                            }
                        }
                        Random seed4 = new Random();
                        Thread.Sleep(seed4.Next(0, 2) * 10);
                        // Jeśli jeden lub oba są zajęte to czeka (lub po skończeniu jedzenia - odpoczynek)
                    }
                
            }

        }
        static void Main(string[] args)
        {
            filozofowie akcje = new filozofowie();

            //Jest 5 filozofów, każdy jest osobnym wątkiem
            Thread Filozof_1 = new Thread(akcje.Filozof);
            Thread Filozof_2 = new Thread(akcje.Filozof);
            Thread Filozof_3 = new Thread(akcje.Filozof);
            Thread Filozof_4 = new Thread(akcje.Filozof);
            Thread Filozof_5 = new Thread(akcje.Filozof);

            Filozof_1.Start(); Filozof_2.Start(); Filozof_3.Start(); Filozof_4.Start(); Filozof_5.Start();

            Filozof_1.Join(); Filozof_2.Join(); Filozof_3.Join(); Filozof_4.Join(); Filozof_5.Join();

            WriteLine("Wszyscy zjedli");
            ReadKey();
        }
    }
}
// zmianne: Liczba filozofów, k - liczba książek, liczba sztućcy, liczba miejsc
// starczy 5

/*
 * 
Problem ucztujących filozofów:
Mogą za każdym razem usiąść przy innym miejscu. Widelce biorą zawsze z sąsiedztwa wybranego nakrycia, ale miejsce wybierają również losowo.

*/


/*
wybór książki-> Wybór miejsca -> zabranie 2 widelcy (z sąsiednich nakryć)


Dla pojedynczego filozofa:


-> wybór książki (Spośród wolnych losowanie)

-> wybór miejsca (Spośród wolnych losowanie)

-> Próba jedzenia (Sprawdzenie, czy oba sąsiednie widelce są wolne)

Jeśli tak, to je i blokuje widelce aż skończy. 

Jeśli nie, to czeka aż będą wolne oba.

Po zjedzeniu odkłada widelce.

*/

//Copyright: Hubert Kieslich