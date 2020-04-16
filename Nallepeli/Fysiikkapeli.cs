using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

/// @author Aapo Anttila
/// @version 15.4.2020

public class Nallepeli : PhysicsGame
{
    ///kuvia
    Image jaapuikko = LoadImage("jaapuikko2");
    Image nallepaa = LoadImage("nallepaa");
    Image[] hunajaKuva = LoadImages("hunaja", "hunaja2", "hunaja3", "hunaja4");
    Image nallekuollutpaa = LoadImage("nallekuollutpaa");
    Shape jaapuikonMuoto;

    public const double liike = 3000.0;
    public double vaikeus = 0.3;

    IntMeter pisteLaskuri;
    ScoreList topLista = new ScoreList(10, false, 0);

    /// <summary>
    /// Pääohjelma
    /// </summary>
    public override void Begin()
    {
        MultiSelectWindow alkuValikko = new MultiSelectWindow("Pelin alkuvalikko",
        "Pelaa", "Vaikeustaso", "Lopeta");
        Add(alkuValikko);
        alkuValikko.AddItemHandler(0, AloitaPeli);
        alkuValikko.AddItemHandler(1, VaikeusTaso);
        alkuValikko.AddItemHandler(2, Exit);

        void VaikeusTaso()
        {
            MultiSelectWindow valikko = new MultiSelectWindow("Tervetuloa peliin",
            "Helppo", "Normaali", "Vaikea");
            valikko.ItemSelected += PainettiinValikonNappia;
            Add(valikko);
            void PainettiinValikonNappia(int valinta)
            {
                switch (valinta)
                {
                    case 0:
                        vaikeus = 0.5;
                        Begin();
                        break;
                    case 1:
                        vaikeus = 0.3;
                        Begin();
                        break;
                    case 2:
                        vaikeus = 0.1;
                        Begin();
                        break;
                }
            }
        }

        void AloitaPeli()
        {
            LuoKentta(vaikeus);
        }


    }


    /// <summary>
    /// Luo kentän 
    /// </summary>
    /// <param name="vaikeustaso">asettaa jääpuikkojen putoamisnopeuden</param>
    public void LuoKentta(double vaikeustaso)
    {
        Level.Background.Color = Color.Black;
        SetWindowSize(1024, 768);
        Level.CreateBorders();
        Gravity = new Vector(0.0, -300.0);
        Surface alaReuna = Surface.CreateBottom(Level);
        Add(alaReuna);
        alaReuna.Tag = "alareuna";
        LuoPistelaskuri();
        LuoNalle();
        topLista = DataStorage.TryLoad<ScoreList>(topLista, "pisteet.xml");
        Timer.CreateAndStart(vaikeustaso, LuoJaapuikkoja);         ///käytetään LuoJaapuikkoja vaikeustason mukaisen ajan välein

        void LuoJaapuikkoja()
        {
                LuoJaapuikko(Level.Top, 100, 150); 
        }


        Timer.CreateAndStart(RandomGen.NextDouble(3, 10), LuoHunajaa);

        void LuoHunajaa()
        {
            LuoHunaja(Level.Top, 50, 50, RandomGen.NextInt(1,4)) ;
        }

        Keyboard.Listen(Key.R, ButtonState.Pressed, AloitaAlusta, "Aloittaa pelin alusta");
        Keyboard.Listen(Key.F2, ButtonState.Pressed, AlkuValikko, "Avaa alkuvalikon");
        Keyboard.Listen(Key.P, ButtonState.Pressed, Pause, "Pysäyttää pelin");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }


    /// <summary>
    /// luodaan jääpuikot
    /// </summary>
    /// <param name="paikka">sijainti mihin jääpuikko luodaan</param>
    /// <param name="leveys">jääpuikon leveys</param>
    /// <param name="korkeus">jääpuikon korkeus</param>
    public void LuoJaapuikko(double paikka, double leveys, double korkeus)
    {
        jaapuikonMuoto = Shape.FromImage(jaapuikko);
        PhysicsObject jaapuikot = new PhysicsObject(leveys, korkeus, jaapuikonMuoto);
        jaapuikot.Image = jaapuikko;
        jaapuikot.Y = paikka;
        jaapuikot.X = RandomGen.NextDouble(Level.Left, Level.Right);
        jaapuikot.Tag = "jaapuikko";
        Add(jaapuikot);
        AddCollisionHandler(jaapuikot, "alareuna", JaapuikkoOsuuMaahan);

        void JaapuikkoOsuuMaahan(PhysicsObject jaapuikot, PhysicsObject kohde)
        {
            ///jääpuikko tuhoutuu osuessaan kentän alarajaan
            pisteLaskuri.Value += 1;
            jaapuikot.Destroy();
        }

    }


    /// <summary>
    /// luodaan hunaja, joka kasvattaa healthia
    /// </summary>
    /// <param name="paikka"></param>
    /// <param name="leveys"></param>
    /// <param name="korkeus"></param>
    public void LuoHunaja(double paikka, double leveys, double korkeus, int a)
    {
        PhysicsObject hunaja = new PhysicsObject(leveys, korkeus, Shape.Rectangle);
        hunaja.Image = hunajaKuva[a];
        hunaja.Y = paikka;
        hunaja.X = RandomGen.NextDouble(Level.Left + 10, Level.Right - 10);
        hunaja.Tag = "hunaja";
        Add(hunaja);
        AddCollisionHandler(hunaja, "alareuna", CollisionHandler.DestroyObject); ///hunaja tuhoutuu osuessaan kentän alarajaan
    }


    /// <summary>
    ///tässä luodaan nalle, joka koostuu päästä, kehosta ja raajoista
    /// </summary>
    public void LuoNalle()
    {
        PhysicsObject paa = new PhysicsObject(100, 72, Shape.Ellipse);
        PhysicsObject keho = new PhysicsObject(100, 125, Shape.Ellipse);
        PhysicsObject jalkavasen = new PhysicsObject(60, 25, Shape.Ellipse);
        PhysicsObject jalkaoikea = new PhysicsObject(60, 25, Shape.Ellipse);
        PhysicsObject oikKasi = new PhysicsObject(50, 25, Shape.Ellipse);
        PhysicsObject vasKasi = new PhysicsObject(50, 25, Shape.Ellipse);

        jalkaoikea.Position = new Vector(keho.X + 80, keho.Y - 30);
        jalkavasen.Position = new Vector(keho.X - 80, keho.Y - 30);
        vasKasi.Position = new Vector(keho.X - 70, keho.Y + 20);
        oikKasi.Position = new Vector(keho.X + 70, keho.Y + 20);
        paa.Position = new Vector(0, keho.Y + 70);

        paa.Image = nallepaa;
        keho.Color = new Color(127, 106, 0);
        jalkavasen.Color = new Color(127, 106, 0);
        jalkaoikea.Color = new Color(127, 106, 0);
        oikKasi.Color = new Color(127, 106, 0);
        vasKasi.Color = new Color(127, 106, 0);

        AxleJoint liitosPaaKeho = new AxleJoint(paa, keho, new Vector(keho.X, keho.Y + 70));
        AxleJoint liitosKehoVasJalka = new AxleJoint(jalkavasen, keho, new Vector(keho.X - 25, keho.Y - 25));
        AxleJoint liitosKehoOikJalka = new AxleJoint(jalkaoikea, keho, new Vector(keho.X + 25, keho.Y - 25));
        AxleJoint liitosOikKasi = new AxleJoint(oikKasi, keho, new Vector(keho.X + 20, keho.Y + 35));
        AxleJoint liitosVasKasi = new AxleJoint(vasKasi, keho, new Vector(keho.X - 20, keho.Y + 35));
        liitosPaaKeho.Softness = 0;
        liitosKehoVasJalka.Softness = 0.0;
        liitosKehoOikJalka.Softness = 0.0;
        liitosOikKasi.Softness = 0.1;
        liitosVasKasi.Softness = 0.1;
        paa.AngularDamping = 0.001;
        keho.AngularDamping = 0.5;

        paa.CollisionIgnoreGroup = 1;
        keho.CollisionIgnoreGroup = 1;

        Add(liitosVasKasi);
        Add(liitosOikKasi);
        Add(liitosPaaKeho);
        Add(liitosKehoVasJalka);
        Add(liitosKehoOikJalka);
        Add(paa);
        Add(keho);
        Add(jalkavasen);
        Add(jalkaoikea);
        Add(oikKasi);
        Add(vasKasi);

        int[] tilastot = { 3, 0, 0 };


        ///aliohjelma määrää mitä tapahtuu kun jääpuikko osuu kehoon tai päähän 
        void Osuma(PhysicsObject tormaaja, PhysicsObject kohde)
        {
            tilastot[0] -= 1;
            tilastot[1] += 1;
            kohde.Destroy();
            if (tilastot[0] == 0)
            {
                liitosPaaKeho.Destroy();
                paa.Image = nallekuollutpaa;
                Timer ajastin = new Timer();
                ajastin.Interval = 0.001;
                ajastin.Timeout += Verta;
                ajastin.Start(200);

                void Verta()
                {
                    LuoVerta(new Vector(keho.X, keho.Y - keho.Height / 2));
                    LuoVerta(new Vector(paa.X, paa.Y - paa.Height / 2));
                }

                int pisteet = Pisteet(tilastot);
                HighScoreWindow topIkkuna = new HighScoreWindow(
                "Parhaat pisteet",
                "Väistit jääpuikkoa " +pisteLaskuri.Value + ", sinuun osui " + tilastot[1] + " jääpuikkoa ja keräsit " + tilastot[2] + " purkkia hunajaa. Piisteesi on %p" ,
                topLista,
                pisteet
                );
                Add(topIkkuna);

                topIkkuna.Closed += delegate (Window ikkuna)
                {
                    DataStorage.Save<ScoreList>(topLista, "pisteet.xml");
                };

                topIkkuna.Closed += delegate (Window ikkuna)
                {
                    AlkuValikko();
                };
                
                
                int Pisteet(int[] tilastot)
                {
                    int pisteet = pisteLaskuri.Value + 20 * tilastot[2];
                    return pisteet; 
                }

            }
        }


        ///kun jääpuikko osuu raajaan, raaja irtoaa kehosta
        void RaajaOsuma(PhysicsObject tormaaja, PhysicsObject kohde)
        {
            if (tormaaja == vasKasi)
            { liitosVasKasi.Destroy(); }
            if (tormaaja == oikKasi)
            { liitosOikKasi.Destroy(); }
            if (tormaaja == jalkaoikea)
            { liitosKehoOikJalka.Destroy(); }
            if (tormaaja == jalkavasen)
            { liitosKehoVasJalka.Destroy(); }

            kohde.Destroy();
            tormaaja.IgnoresCollisionResponse = true;
            Timer ajastin = new Timer();
            ajastin.Interval = 0.001;
            ajastin.Timeout += Verta;
            ajastin.Start(1000);
            void Verta()
            {
                if (tormaaja == vasKasi) { LuoVerta(new Vector(keho.X - 25, keho.Y + 35)); }
                if (tormaaja == oikKasi) { LuoVerta(new Vector(keho.X + 25, keho.Y + 35)); }
                if (tormaaja == jalkavasen) { LuoVerta(new Vector(keho.X - 35, keho.Y - 25)); }
                if (tormaaja == jalkaoikea) { LuoVerta(new Vector(keho.X + 35, keho.Y - 25)); }

            }
        }


        void HunajaOsuma(PhysicsObject tormaaja, PhysicsObject kohde)
        {
            ///hunaja kasvattaa healthia osuessaan päähän tai kehoon
            tilastot[0] += 1;
            tilastot[2] += 1;
            kohde.Destroy();
        }


        AddCollisionHandler(vasKasi, "jaapuikko", RaajaOsuma);
        AddCollisionHandler(oikKasi, "jaapuikko", RaajaOsuma);
        AddCollisionHandler(jalkaoikea, "jaapuikko", RaajaOsuma);
        AddCollisionHandler(jalkavasen, "jaapuikko", RaajaOsuma);
        AddCollisionHandler(paa, "hunaja", HunajaOsuma);
        AddCollisionHandler(keho, "hunaja", HunajaOsuma);
        AddCollisionHandler(paa, "jaapuikko", Osuma);
        AddCollisionHandler(keho, "jaapuikko", Osuma);

        Keyboard.Listen(Key.Right, ButtonState.Down, Liikuta, " Liikuta pelaajaa oikealle", paa, liike, 0.0);
        Keyboard.Listen(Key.Left, ButtonState.Down, Liikuta, " Liikuta pelaajaa vasemmalle", paa, -liike, 0.0);
        Keyboard.Listen(Key.Up, ButtonState.Down, Liikuta, " Liikuta pelaajaa ylös", paa, 0.0, liike * 4 / 3);
        Keyboard.Listen(Key.Down, ButtonState.Down, Liikuta, " Liikuta pelaajaa alas", paa, 0.0, -liike / 3);
    }


    /// <summary>
    /// tässä luodaan veri 
    /// </summary>
    /// <param name="paikka">sijainti mihin veri luodaan</param>
    public void LuoVerta(Vector paikka)
    {
        PhysicsObject veri = new PhysicsObject(1, 1, Shape.Circle);
        veri.Color = Color.Red;
        veri.Position = paikka;
        Add(veri);
        AddCollisionHandler(veri, "alareuna", CollisionHandler.DestroyObject);
        veri.IgnoresCollisionResponse = true;
        veri.Hit(new Vector(RandomGen.NextDouble(-200, 200), RandomGen.NextDouble(-200, 200)));
    }


    /// <summary>
    /// luodaan pistelaskuri joka ilmoittaa montako jääpuikkoa on osunut maahan
    /// </summary>
    public void LuoPistelaskuri()
    {
        pisteLaskuri = new IntMeter(0);
        Label pisteNaytto = new Label();
        pisteNaytto.X = Screen.Left + 100;
        pisteNaytto.Y = Screen.Top - 100;
        pisteNaytto.TextColor = Color.Black;
        pisteNaytto.Color = Color.White;
        pisteNaytto.BindTo(pisteLaskuri);
        Add(pisteNaytto);
    }


    /// <summary>
    /// liikkeen tekevä aliohjelma
    /// </summary>
    /// <param name="liikuteltavaOlio">olio jota liikutetaan</param>
    /// <param name="suunta">liikkeen suuruus x suunnassa</param>
    /// <param name="suunta2">liikkeen suuruus y suunnassa</param>
    public void Liikuta(PhysicsObject liikuteltavaOlio, double suunta, double suunta2)
    {
        liikuteltavaOlio.Push(new Vector(suunta, suunta2));
    }


    /// <summary>
    /// aloittaa pelin uudestaan
    /// </summary>
    public void AloitaAlusta()
    {
        ClearAll();
        LuoKentta(vaikeus);
    }


    /// <summary>
    /// avaa alkuvalikon
    /// </summary>
    public void AlkuValikko()
    {
        ClearAll();
        Begin();
    }


}
