using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

public class Nallepeli : PhysicsGame
{
    ///kuvia
    
    Image jaapuikko = LoadImage("jaapuikko2");
    Shape jaapuikonMuoto;
    Image nallepaa = LoadImage("nallepaa");
    IntMeter pisteLaskuri;


    public override void Begin()
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
        Timer.CreateAndStart(0.3, LuoJaapuikkoja); ///käytetään LuoJaapuikkoja aliohjelmaa 0.3 sekunnin välein

        void LuoJaapuikkoja()
        {
            for (int i = 0; i < 1; i++)   ///luo yhden jääpuikon
            {
                LuoJaapuikko(Level.Top, 100, 150);

            }
        }

        Timer.CreateAndStart(RandomGen.NextDouble(3, 10), LuoHunajaa);

        void LuoHunajaa()
        {
            for (int i = 0; i < 1; i++)
            {
                LuoHunaja(Level.Top, 50, 50);

            }
        }

        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }


    public void Liikuta(PhysicsObject liikuteltavaOlio, double suunta, double suunta2)
    {
        ///liikeen tekevä aliohjelma
        liikuteltavaOlio.Push(new Vector(suunta, suunta2));
    }


    public void LuoJaapuikko(double paikka, double leveys, double korkeus)
    {

        ///luodaan jääpuikkoja
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
            ///kun jääpuikko osuu kentän alarajaan jääpuikko tuhoutuu
            pisteLaskuri.Value += 1;
            jaapuikot.Destroy();
        }

    }


    public void LuoHunaja(double paikka, double leveys, double korkeus)
    {
        ///luodaan hunajaa
        PhysicsObject hunaja = new PhysicsObject(leveys, korkeus, Shape.Rectangle);
        hunaja.Image = LoadImage("hunaja");
        hunaja.Y = paikka;
        hunaja.X = RandomGen.NextDouble(Level.Left + 10, Level.Right - 10);
        hunaja.Tag = "hunaja";
        Add(hunaja);
        AddCollisionHandler(hunaja, "alareuna", CollisionHandler.DestroyObject); ///hunaja tuhoutuu osuessaan kentän alarajaan
    }


    void LuoPistelaskuri()
    {
        ///tässä luodaan pistelaskuri
        pisteLaskuri = new IntMeter(0);

        Label pisteNaytto = new Label();
        pisteNaytto.X = Screen.Left + 100;
        pisteNaytto.Y = Screen.Top - 100;
        pisteNaytto.TextColor = Color.Black;
        pisteNaytto.Color = Color.White;

        pisteNaytto.BindTo(pisteLaskuri);
        Add(pisteNaytto);
    }


    public void LuoNalle()
    {
        ///tässä luodaan nalle, joka koostuu päästä, kehosta ja raajoista

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

        paa.Image = nallepaa;
        paa.Position = new Vector(0, keho.Y + 70);
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
        paa.AngularDamping = 0.05;
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

        int health = 3;

        AddCollisionHandler(paa, "jaapuikko", Osuma);
        AddCollisionHandler(keho, "jaapuikko", Osuma);
        void Osuma(PhysicsObject tormaaja, PhysicsObject kohde)
        {
            health--;
            kohde.Destroy();
            if (health < 1)
            {
                liitosPaaKeho.Destroy();
                Timer ajastin = new Timer();
                ajastin.Interval = 0.001;
                ajastin.Timeout += Verta;
                ajastin.Start(200);
                void Verta()
                {
                    LuoVerta(new Vector(keho.X, keho.Y - keho.Height/2));
                    LuoVerta(new Vector(paa.X, paa.Y  - paa.Height/2));

                }
                MessageDisplay.Add("Hävisit pelin");
            }

        }
        


        AddCollisionHandler(vasKasi, "jaapuikko", KasiOsuma);
        AddCollisionHandler(oikKasi, "jaapuikko", KasiOsuma);
        AddCollisionHandler(jalkaoikea, "jaapuikko", KasiOsuma);
        AddCollisionHandler(jalkavasen, "jaapuikko", KasiOsuma);

        void KasiOsuma(PhysicsObject tormaaja, PhysicsObject kohde)
        {
            ///kun jääpuikko osuu käteen, käsi irtoaa kehosta
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
            {   if (tormaaja == vasKasi) { LuoVerta(new Vector(keho.X - 25, keho.Y + 35)); }
                if (tormaaja == oikKasi) { LuoVerta(new Vector(keho.X + 25, keho.Y + 35)); }
                if (tormaaja == jalkavasen) { LuoVerta(new Vector(keho.X - 35, keho.Y - 25)); }
                if (tormaaja == jalkaoikea) { LuoVerta(new Vector(keho.X + 35, keho.Y - 25)); }  

            }
        }


        AddCollisionHandler(paa, "hunaja", HunajaOsuma);
        AddCollisionHandler(keho, "hunaja", HunajaOsuma);

        void HunajaOsuma(PhysicsObject tormaaja, PhysicsObject kohde)
        {
                health++;
                kohde.Destroy();
        }
 

            Keyboard.Listen(Key.Right, ButtonState.Down, Liikuta, " Liikuta pelaajaa oikealle", paa, 2000.0, 0.0);
            Keyboard.Listen(Key.Left, ButtonState.Down, Liikuta, " Liikuta pelaajaa vasemmalle", paa, -2000.0, 0.0);
            Keyboard.Listen(Key.Up, ButtonState.Down, Liikuta, " Liikuta pelaajaa ylös", paa, 0.0, 3000.0);
            Keyboard.Listen(Key.Down, ButtonState.Down, Liikuta, " Liikuta pelaajaa alas", paa, 0.0, -500.0);
        }
    

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

}
