using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

public class Nallepeli : PhysicsGame
{
    ///kuvia
    Image nalle = LoadImage("nalle");
    Shape nallenMuoto;
    Image jaapuikko = LoadImage("jaapuikko2");
    Shape jaapuikonMuoto;


    int health;
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
        LuoPelaaja(204, 200);
        Timer.CreateAndStart(0.2, LuoJaapuikkoja); ///käytetöön LuoJaapuikkoja aliohjelmaa 0.2 sekunnin välein

        void LuoJaapuikkoja()
        {
            for (int i = 0; i < 1; i++)   ///luo yhden jääpuikon
            {
                LuoJaapuikko(Level.Top, 100, 150);

            }
        }

        Timer.CreateAndStart(RandomGen.NextDouble(3,10), LuoHunajaa);

        void LuoHunajaa()
        {
            for (int i = 0; i < 1; i++)
            {
                LuoHunaja(Level.Top, 50, 50);

            }
        }

        // Kirjoita ohjelmakoodisi tähän

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }
    public void LuoPelaaja(double leveys, double korkeus)
    {
        ///aliohjelma luo pelaajan (nallen)

        nallenMuoto = Shape.FromImage(nalle);
        PhysicsObject pelaaja = new PhysicsObject(leveys, korkeus, nallenMuoto);
        pelaaja.Image = nalle;
        pelaaja.Tag = "nalle";
        Add(pelaaja);
        AddCollisionHandler(pelaaja, "piikki", OtaVastaanOsuma);
        health = 3;
        void OtaVastaanOsuma(PhysicsObject pelaaja, PhysicsObject kohde)
        {
            ///kun jääpuikko "piikki" osuu pelaajaan, health vähenee ja jääpuikko tuhoutuu
            if (health == 1)
            pelaaja.Destroy();            
            health--;
            kohde.Destroy();

        }

        AddCollisionHandler(pelaaja, "hunaja", OtaVastaanHunaja);
        void OtaVastaanHunaja(PhysicsObject pelaaja, PhysicsObject kohde)
        {
            ///kun hunaja osuu pelaajaan, health kasvaa yhdellä ja hunaja tuhoutuu
            health++;
            kohde.Destroy();

        }
        Keyboard.Listen(Key.Right, ButtonState.Down, Liikuta, " Liikuta pelaajaa oikealle", pelaaja, 500.0, 0.0);
        Keyboard.Listen(Key.Left, ButtonState.Down, Liikuta, " Liikuta pelaajaa vasemmalle", pelaaja, -500.0, 0.0);
        Keyboard.Listen(Key.Up, ButtonState.Down, Liikuta, " Liikuta pelaajaa ylös", pelaaja, 0.0, 500.0);
        Keyboard.Listen(Key.Down, ButtonState.Down, Liikuta, " Liikuta pelaajaa alas", pelaaja, 0.0, -500.0);

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
        jaapuikot.Tag = "piikki";
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


}
