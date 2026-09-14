using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PHDModManager
{
    public partial class MainForm : Form
    {
        private class CategoryGroup
        {
            public Panel Header;
            public Panel Detail;
        }

        private class ModItem
        {
            public string Key;
            public string DisplayName;
            public bool Instalado;

            // True si el archivo ya existe en algún lado (carpeta activa o
            // backup). Para la mayoría de las categorías esto siempre es true
            // porque PrepararItems/PrepararItemsScripts solo agregan el item
            // si ya encontraron el archivo. HUD Player es la excepción: ahora
            // el item aparece SIEMPRE, así que puede valer false (todavía no
            // instalado) y la fila muestra un botón "Instalar" en vez del
            // toggle + botón de borrado.
            public bool Presente = true;
        }

        private readonly List<CategoryGroup> categorias = new List<CategoryGroup>();
        private const int Spacing = 10;

        // Estado auxiliar de PoblarLista: permite llamarla varias veces (recarga)
        // sin duplicar los eventos de búsqueda / toggle maestro por categoría.
        private readonly Dictionary<Panel, bool[]> _flagsSincronizacion = new Dictionary<Panel, bool[]>();
        private readonly HashSet<Panel> _listasEnganchadas = new HashSet<Panel>();

        private string carpetaImages;
        private string carpetaBackup;
        private string carpetaScripts;
        private string carpetaBackupScripts;

        private const string ArchivoHudCustomizable = "HudPlayerCustomizable.gsc";

        // Nombre completo del recurso embebido (namespace por defecto del
        // proyecto + carpeta "Recursos" + nombre de archivo, con puntos en
        // vez de barras). Si el namespace por defecto del proyecto NO es
        // "PHDModManager", o si el archivo se pone en otra carpeta, hay que
        // ajustar esta cadena. Para confirmar el nombre exacto en caso de
        // duda, se puede recorrer Assembly.GetExecutingAssembly().GetManifestResourceNames()
        // una vez y ver qué imprime.
        private const string RecursoHudCustomizable = "PHDModManager.Recursos.HudPlayerCustomizable.gsc";

        // Nombre fijo del material/ícono del HUD (character portrait) que usan
        // las 4 funciones de HudPlayerCustomizable.gsc vía setshader(...).
        // Este archivo NO existe por defecto en carpetaImages (el original
        // vive compilado dentro de los assets del juego); el botón "Cambiar
        // ícono del HUD" simplemente copia el .iwi elegido con este nombre,
        // sobrescribiendo lo que hubiera antes. El código del .gsc nunca se
        // toca para esto: siempre sigue apuntando a este mismo nombre.
        private const string NombreArchivoIconoHud = "zombies_rank_3_ded.iwi";

        private static readonly string RutaConfig = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PHDModManager", "config.txt"
        );

        private static readonly string RutaConfigIdioma = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PHDModManager", "idioma.txt"
        );

        private static readonly string RutaLog = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PHDModManager", "log.txt"
        );

        // ============================
        // DICCIONARIOS DE NOMBRES
        // ============================
        private static readonly Dictionary<string, string> NombresArmas = new Dictionary<string, string>
        {
            { "dsr50_zm", "DSR-50" },
            { "barretm82_zm", "Barrett M82A1" },
            { "svu_zm", "SVU-AS" },
            { "ballista_zm", "Ballista" },
            { "ak74u_zm", "AK74u" },
            { "mp5k_zm", "MP5K" },
            { "pdw57_zm", "PDW-57" },
            { "qcw05_zm", "Chicom CQB" },
            { "thompson_zm", "Thompson" },
            { "uzi_zm", "Uzi" },
            { "mp40_zm", "MP40" },
            { "mp40_stalker_zm", "MP40 (Stalker)" },
            { "evoskorpion_zm", "Skorpion EVO III" },
            { "fnfal_zm", "FAL" },
            { "m14_zm", "M14" },
            { "saritch_zm", "SMR" },
            { "m16_zm", "M16" },
            { "tar21_zm", "TAR-21" },
            { "gl_tar21_zm", "TAR-21 (Grenade Launcher)" },
            { "galil_zm", "Galil" },
            { "an94_zm", "AN-94" },
            { "type95_zm", "Type 25" },
            { "xm8_zm", "M8A1" },
            { "ak47_zm", "AK-47" },
            { "mp44_zm", "MP-44" },
            { "scar_zm", "SCAR-H" },
            { "hk416_zm", "M27" },
            { "870mcs_zm", "R870 MCS" },
            { "rottweil72_zm", "Olympia" },
            { "saiga12_zm", "S12" },
            { "srm1216_zm", "M1216" },
            { "ksg_zm", "KSG" },
            { "lsat_zm", "LSAT" },
            { "hamr_zm", "HAMR" },
            { "rpd_zm", "RPD" },
            { "minigun_alcatraz_zm", "Death Machine" },
            { "mg08_zm", "MG08" },
            { "m1911_zm", "M1911" },
            { "rnma_zm", "Remington New Model Army" },
            { "judge_zm", "Executioner" },
            { "kard_zm", "Kap-40" },
            { "fiveseven_zm", "Five-Seven" },
            { "fivesevendw_zm", "Five-Seven (Dual Wield)" },
            { "beretta93r_zm", "B23R" },
            { "python_zm", "Python" },
            { "c96_zm", "Mauser C96" },
            { "usrpg_zm", "RPG" },
            { "m32_zm", "War Machine" },
            { "knife_ballistic_zm", "Ballistic Knife" },
            { "knife_ballistic_bowie_zm", "Ballistic Knife (Bowie)" },
            { "knife_ballistic_no_melee_zm", "Ballistic Knife (No Melee)" },
            { "ray_gun_zm", "Ray Gun" },
            { "raygun_mark2_zm", "Ray Gun Mark II" },
            { "slowgun_zm", "Paralyzer" },
            { "slipgun_zm", "Sliquifier" },
            { "blundergat_zm", "Blundergat" },
            { "blundersplat_zm", "Acidgat" },
            { "staff_fire_zm", "Fire Staff" },
            { "staff_water_zm", "Ice Staff" },
            { "staff_air_zm", "Wind Staff" },
            { "staff_lightning_zm", "Lightning Staff" },
            { "staff_revive_zm", "Staff of Revive" },
            { "cymbal_monkey_zm", "Monkey Bomb" },
            { "frag_grenade_zm", "Frag Grenade" },
            { "claymore_zm", "Claymore" },
            { "time_bomb_zm", "Time Bomb" },
            { "sticky_grenade_zm", "Semtex" },
            { "emp_grenade_zm", "EMP Grenade" },
            { "beacon_zm", "Gersh Device" },
            { "bouncing_tomahawk_zm", "Tomahawk" },
            { "knife_zm", "Default Knife" },
            { "bowie_knife_zm", "Bowie Knife" },
            { "tazer_knuckles_zm", "Galvaknuckles" },
            { "spoon_zm_alcatraz", "Spoon" },
            { "spork_zm_alcatraz", "Spork" },
            { "one_inch_punch_zm", "One Inch Punch" },
            { "equip_turbine_zm", "Turbine" },
            { "equip_springpad_zm", "Trample Steam" },
            { "equip_subwoofer_zm", "Sub-Woofer" },
            { "equip_headchopper_zm", "Head Chopper" },
            { "jetgun_zm", "Jet Gun" },
            { "equip_electrictrap_zm", "Electric Trap" },
            { "equip_turret_zm", "Turret" },
            { "equip_dieseldrone_zm", "Maxis Drone" },
            { "riotshield_zm", "Shield (Buried)" },
            { "alcatraz_shield_zm", "Shield (Mob of the Dead)" },
            { "tomb_shield_zm", "Shield (Origins)" },
        };

        private static readonly Dictionary<string, string> NombresPerks = new Dictionary<string, string>
        {
            { "specialty_armorvest", "Juggernog" },
            { "specialty_quickrevive", "Quick Revive" },
            { "specialty_fastreload", "Speed Cola" },
            { "specialty_rof", "Double Tap II" },
            { "specialty_longersprint", "Stamin-Up" },
            { "specialty_additionalprimaryweapon", "Mule Kick" },
            { "specialty_nomotionsensor", "Vulture Aid" },
            { "specialty_finalstand", "Who's Who" },
            { "specialty_grenadepulldeath", "Electric Cherry" },
            { "specialty_ads_zombies", "Deadshot Daiquiri" },
            { "specialty_flakjacket", "PhD Flopper" },
            { "specialty_scavenger", "Tombstone Soda" },
        };

        private static readonly Dictionary<string, string> NombresGuantes = new Dictionary<string, string>
        {
            { "guante_engineer", "Marlton" },
            { "guante_farmergirl", "Misty" },
            { "guante_oldman", "Russman" },
            { "guante_reporter", "Samuel Stuhlinger" },
            { "guante_takeo", "Takeo Masaki" },
            { "guante_richtofen", "Edward Richtofen" },
            { "guante_nikola", "Nikolai Belinski" },
            { "guante_dempsey", "Tank Dempsey" },
            { "guante_deluca", "Sal DeLuca" },
            { "guante_oleary", "Finn O'Leary" },
            { "guante_handsome", "Billy Handsome" },
            { "guante_arlington", "Arlington" },
        };

        // ============================
        // TOKENS PARA DETECTAR ARCHIVOS
        // ============================
        private static readonly Dictionary<string, string> TokenArchivo = new Dictionary<string, string>
        {
            { "dsr50_zm", "sniper_dsr50" },
            { "barretm82_zm", "sniper_m82" },
            { "svu_zm", "sniper_svu" },
            { "ballista_zm", "sniper_ballist" },
            { "ak74u_zm", "smg_ak74u" },
            { "mp5k_zm", "smg_mp5" },
            { "pdw57_zm", "smg_pdw57" },
            { "qcw05_zm", "smg_chicom" },
            { "thompson_zm", "zmb_thompson" },
            { "uzi_zm", "smg_uzi" },
            { "mp40_zm", "smg_mp40" },
            { "evoskorpion_zm", "smg_scorpion" },
            { "fnfal_zm", "ar_fal" },
            { "m14_zm", "ar_m14" },
            { "saritch_zm", "ar_sig556" },
            { "galil_zm", "ar_galil" },
            { "an94_zm", "ar_an94" },
            { "type95_zm", "ar_type95" },
            { "xm8_zm", "ar_xm8" },
            { "ak47_zm", "ar_ak47" },
            { "mp44_zm", "ar_stg44" },
            { "hk416_zm", "ar_hk416" },
            { "870mcs_zm", "shotty_870mcs" },
            { "rottweil72_zm", "shotty_olympia" },
            { "saiga12_zm", "shotty_saiga" },
            { "srm1216_zm", "shotty_srm1216" },
            { "ksg_zm", "shotty_ksg" },
            { "lsat_zm", "lmg_lsat" },
            { "hamr_zm", "lmg_hamr" },
            { "rpd_zm", "lmg_rpd" },
            { "minigun_alcatraz_zm", "minigun" },
            { "mg08_zm", "zmb_mg08" },
            { "m1911_zm", "pistol_m1911" },
            { "rnma_zm", "pistol_rnma" },
            { "judge_zm", "pistol_judge" },
            { "kard_zm", "pistol_kard" },
            { "fiveseven_zm", "pistol_fivesev" },
            { "beretta93r_zm", "pistol_b2023r" },
            { "python_zm", "pistol_python" },
            { "usrpg_zm", "launch_usrpg" },
            { "m32_zm", "launch_m32" },
            { "knife_ballistic_zm", "ballistic_knife" },
            { "ray_gun_zm", "zmb_raygun_col" },
            { "raygun_mark2_zm", "zmb_raygun2" },
            { "slowgun_zm", "zmb_slowgun" },
            { "slipgun_zm", "zmb_slipgun" },
            { "blundergat_zm", "zmb_blundergat_col" },
            { "blundersplat_zm", "zmb_blundergat_acid" },
            { "cymbal_monkey_zm", "monkey_bomb" },
            { "frag_grenade_zm", "grenade_frag" },
            { "time_bomb_zm", "zmb_timebomb" },
            { "sticky_grenade_zm", "grenade_semtex" },
            { "emp_grenade_zm", "grenade_emp" },
            { "beacon_zm", "zmb_beacon" },
            { "bouncing_tomahawk_zm", "zmb_tomahawk" },
            { "knife_zm", "knife_base" },
            { "tazer_knuckles_zm", "taser_knuckles" },
            { "equip_subwoofer_zm", "zmb_subwoofer" },
            { "equip_headchopper_zm", "zmb_chopper" },
            { "jetgun_zm", "zmb_jet_gun" },
            { "tomb_shield_zm", "zmb_shield_col" },
            { "alcatraz_shield_zm", "shield_dlc2" },
            { "riotshield_zm", "shield_dlc4" },
            { "specialty_armorvest", "specialty_juggernaut_zombies" },
            { "specialty_quickrevive", "specialty_quickrevive_zombies" },
            { "specialty_fastreload", "specialty_fastreload_zombies" },
            { "specialty_rof", "specialty_doubletap_zombies" },
            { "specialty_longersprint", "specialty_marathon_zombies" },
            { "specialty_additionalprimaryweapon", "specialty_mulekick_zombies" },
            { "specialty_nomotionsensor", "specialty_vulture_zombies" },
            { "specialty_finalstand", "specialty_whosho_zombies" },
            { "specialty_grenadepulldeath", "specialty_cherry_zombies" },
            { "specialty_scavenger", "specialty_tombstone_zombies" },
            { "specialty_ads_zombies", "specialty_ads_zombies" },
            { "specialty_flakjacket", "specialty_divetonuke_zombies" },
            { "guante_engineer", "zom_engineer" },
            { "guante_farmergirl", "zom_farmergirl" },
            { "guante_oldman", "zom_oldman" },
            { "guante_reporter", "zom_reporter" },
            { "guante_takeo", "zom_takeo" },
            { "guante_richtofen", "zom_richtofen" },
            { "guante_nikola", "zom_nikola" },
            { "guante_dempsey", "zom_dempsey" },
            { "guante_deluca", "zom_deluca" },
            { "guante_oleary", "zom_oleary" },
            { "guante_handsome", "zom_handsome" },
            { "guante_arlington", "zom_arlington" },
        };

        public MainForm()
        {
            InitializeComponent();

            AplicarTemaOscuro();
            AplicarTemaBarraTitulo();
            AplicarTemaBarrasDesplazamiento();

            // Cargar y aplicar el idioma guardado ANTES de armar el acordeón,
            // para que todo aparezca ya en el idioma correcto desde el arranque.
            Textos.IdiomaActual = CargarIdiomaGuardado();
            AplicarIdioma();

            ConfigurarAcordeon();
        }

        // ============================
        // TEMA OSCURO (paleta estilo Windows 10 Dark)
        // ============================
        private static readonly Color ColorFondo = Color.FromArgb(32, 32, 32);
        private static readonly Color ColorPanel = Color.FromArgb(43, 43, 43);
        private static readonly Color ColorControlInterno = Color.FromArgb(51, 51, 51);
        private static readonly Color ColorBorde = Color.FromArgb(63, 63, 63);
        private static readonly Color ColorTexto = Color.FromArgb(241, 241, 241);
        private static readonly Color ColorTextoSecundario = Color.FromArgb(155, 155, 155);
        private static readonly Color ColorBotonFondo = Color.FromArgb(60, 60, 60);
        private static readonly Color ColorBotonHover = Color.FromArgb(78, 78, 78);
        private static readonly Color ColorAcento = Color.FromArgb(0, 120, 215);
        private static readonly Color ColorEliminar = Color.FromArgb(232, 90, 90);

        // Aplica la paleta oscura a todos los controles estáticos del
        // diseñador. Las filas que se arman dinámicamente en PoblarLista se
        // pintan ahí mismo con las mismas constantes, porque no existen
        // todavía en este punto (se crean recién al llamar RecargarTodo).
        private void AplicarTemaOscuro()
        {
            BackColor = ColorFondo;

            foreach (var cabecera in new[] { PanelWeapon, PanelPerks, PanelGloves, PanelScripts, PanelHud })
                TemarPanelCabecera(cabecera);

            PanelToolbar.BackColor = ColorPanel;

            foreach (var detalle in new[] { DetailWeapons, DetailPerks, DetailGlove, DetailScripts, DetailHud })
                TemarPanelDetalle(detalle);

            foreach (var lista in new[] { ItemsWeapons, ItemsPerks, ItemsGloves, ItemsScripts, ItemsHud })
                lista.BackColor = ColorControlInterno;

            foreach (var caja in new[] { SearchWeapons, SearchPerks, SearchGloves, SearchScripts, SearchHud })
                TemarTextBox(caja);

            foreach (var boton in new Button[] { BtnRecargar, BtnCambiarCarpeta, BtnAgregarMod, BtnIniciarPlutonium, BtnIdioma, BtnEditarPosicionHud, BtnCambiarIconoHud, BtnReinstalarHud })
                TemarBoton(boton);

            foreach (var icono in new[] { IconSearchWeapons, IconSearchPerks, IconSearchGloves, IconSearchScripts, IconSearchHud })
            {
                icono.ForeColor = ColorTextoSecundario;
                icono.BackColor = Color.Transparent;
            }

            LblVersion.ForeColor = ColorTextoSecundario;
        }

        private void TemarPanelCabecera(Panel panel)
        {
            panel.BackColor = ColorPanel;
            foreach (Control c in panel.Controls)
            {
                if (c is Label lbl)
                {
                    lbl.ForeColor = ColorTexto;
                    lbl.BackColor = Color.Transparent;
                }
            }
        }

        private void TemarPanelDetalle(Panel panel)
        {
            panel.BackColor = ColorFondo;
        }

        private void TemarTextBox(TextBox caja)
        {
            caja.BackColor = ColorControlInterno;
            caja.ForeColor = ColorTexto;
            caja.BorderStyle = BorderStyle.FixedSingle;
        }

        private void TemarBoton(Button boton)
        {
            boton.FlatStyle = FlatStyle.Flat;
            boton.BackColor = ColorBotonFondo;
            boton.ForeColor = ColorTexto;
            boton.FlatAppearance.BorderColor = ColorBorde;
            boton.FlatAppearance.MouseOverBackColor = ColorBotonHover;
            boton.FlatAppearance.MouseDownBackColor = ColorAcento;
        }

        private void ConfigurarAcordeon()
        {
            AgregarCategoria(PanelWeapon, DetailWeapons, ExpandButtonWeapons, 120);
            AgregarCategoria(PanelPerks, DetailPerks, ExpandButtonPerks, 140);
            AgregarCategoria(PanelGloves, DetailGlove, ExpandButtonGloves, 120);
            AgregarCategoria(PanelScripts, DetailScripts, ExpandButtonScripts, 90);
            // 240: antes era 210, pero ahora DetailHud también contiene el
            // botón "Restaurar HUD original empaquetado" además de "Editar
            // posición del HUD", "Cambiar ícono del HUD", el buscador y la
            // lista.
            AgregarCategoria(PanelHud, DetailHud, ExpandButtonHud, 240);

            RepositionAll();

            string carpetaBaseT6 = ResolverCarpetaBaseT6();
            if (carpetaBaseT6 == null)
            {
                Application.Exit();
                return;
            }

            carpetaImages = Path.Combine(carpetaBaseT6, "images");
            carpetaScripts = Path.Combine(carpetaBaseT6, "scripts", "zm");

            carpetaBackup = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "PHDModManager", "DisabledSkins"
            );
            Directory.CreateDirectory(carpetaBackup);

            carpetaBackupScripts = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "PHDModManager", "DisabledScripts"
            );
            Directory.CreateDirectory(carpetaBackupScripts);

            RecargarTodo();
        }

        // ============================
        // IDIOMA (ES/EN) — diccionario en código, sin .resx
        // ============================
        private Idioma CargarIdiomaGuardado()
        {
            try
            {
                if (File.Exists(RutaConfigIdioma))
                {
                    string contenido = File.ReadAllText(RutaConfigIdioma).Trim();
                    if (contenido.Equals("EN", StringComparison.OrdinalIgnoreCase))
                        return Idioma.Ingles;
                }
            }
            catch (Exception ex)
            {
                RegistrarError("CargarIdiomaGuardado", ex);
            }
            return Idioma.Espanol;
        }

        private void GuardarIdioma(Idioma idioma)
        {
            try
            {
                string carpetaConfig = Path.GetDirectoryName(RutaConfigIdioma);
                Directory.CreateDirectory(carpetaConfig);
                File.WriteAllText(RutaConfigIdioma, idioma == Idioma.Ingles ? "EN" : "ES");
            }
            catch (Exception ex)
            {
                RegistrarError("GuardarIdioma", ex);
            }
        }

        // Aplica el idioma actual a todos los controles de UI estática
        // (labels de categoría, botones de la barra, placeholders de
        // búsqueda). Los nombres de armas/perks/guantes individuales NO se
        // tocan acá porque son nombres propios del juego, no texto de UI.
        private void AplicarIdioma()
        {
            WeaponLabel.Text = Textos.T("CatArmas");
            PerksLabel.Text = Textos.T("CatPerks");
            Gloves.Text = Textos.T("CatGuantes");
            ScriptsLabel.Text = Textos.T("CatScripts");
            HudLabel.Text = Textos.T("CatHud");

            BtnRecargar.Text = Textos.T("BtnRecargar");
            BtnCambiarCarpeta.Text = Textos.T("BtnCambiarCarpeta");
            BtnAgregarMod.Text = Textos.T("BtnAgregarMod");
            BtnIdioma.Text = Textos.IdiomaActual == Idioma.Espanol ? "EN" : "ES";
            BtnEditarPosicionHud.Text = Textos.T("BtnEditarPosicionHud");
            BtnCambiarIconoHud.Text = Textos.T("BtnCambiarIconoHud");
            BtnReinstalarHud.Text = Textos.T("BtnReinstalarHud");
            BtnIniciarPlutonium.Text = Textos.T("BtnIniciarPlutonium");

            SetPlaceholder(SearchWeapons, Textos.T("PlaceholderBuscarArma"));
            SetPlaceholder(SearchPerks, Textos.T("PlaceholderBuscarPerk"));
            SetPlaceholder(SearchGloves, Textos.T("PlaceholderBuscarGuante"));
            SetPlaceholder(SearchScripts, Textos.T("PlaceholderBuscarScript"));
            SetPlaceholder(SearchHud, Textos.T("PlaceholderBuscarHud"));
        }

        private void BtnIdioma_Click(object sender, EventArgs e)
        {
            Textos.IdiomaActual = Textos.IdiomaActual == Idioma.Espanol ? Idioma.Ingles : Idioma.Espanol;
            GuardarIdioma(Textos.IdiomaActual);
            AplicarIdioma();
        }

        // Placeholder nativo de TextBox vía Win32 (EM_SETCUEBANNER). Se usa en
        // vez de manejar foco/blur a mano, y desaparece solo al escribir.
        private const int EM_SETCUEBANNER = 0x1501;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);

        // ============================
        // BARRAS DE DESPLAZAMIENTO ESTILO EXPLORADOR (dark mode)
        // ============================
        // SetWindowTheme con la clase "DarkMode_Explorer" hace que las
        // scrollbars nativas de Win32 (las que usan Form/Panel cuando
        // AutoScroll = true) se dibujen igual que en el Explorador de
        // archivos en modo oscuro: delgadas, sin flechas remarcadas y con
        // los mismos grises. Requiere Application.EnableVisualStyles() (ya
        // está en Program.cs) y Windows 10 1809+ / Windows 11; en versiones
        // más viejas simplemente no tiene efecto y queda la scrollbar clásica.
        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        private static extern int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);

        // ============================
        // BARRA DE TÍTULO OSCURA + TRASLÚCIDA (DWM, Windows 11 22000+)
        // ============================
        // DWMWA_USE_IMMERSIVE_DARK_MODE pone la barra de título en modo
        // oscuro. DWMWA_SYSTEMBACKDROP_TYPE = Mica es lo que da el efecto
        // traslúcido/difuminado como en el Explorador de archivos.
        //
        // IMPORTANTE: NO se fuerza DWMWA_CAPTION_COLOR. Si se fija un color
        // sólido ahí, Windows pinta la barra de título plana y tapa el
        // efecto Mica - no se pueden combinar. Dejando que Windows elija el
        // color (según el tema del sistema) es la única forma de que se vea
        // difuminado.
        //
        // LIMITACIÓN: esto solo difumina la BARRA DE TÍTULO (zona no
        // cliente). El resto de la ventana (los paneles oscuros que
        // dibujamos con AplicarTemaOscuro) sigue siendo opaco: WinForms
        // pinta sus controles con un color sólido, y Mica solo se ve donde
        // no hay nada pintado encima. Para que TODA la ventana se vea
        // traslúcida como el fondo del Explorador (detrás de la lista de
        // archivos) haría falta reescribir cómo se pintan los paneles para
        // dejar huecos transparentes — mucho más trabajo, y muchos WinForms
        // no soportan bien ese truco. Avisame si querés que lo intentemos.
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        private const int DWMWA_BORDER_COLOR = 34;
        private const int DWMWA_SYSTEMBACKDROP_TYPE = 38;
        private const int DWMSBT_MAINWINDOW = 2; // Mica

        [DllImport("dwmapi.dll")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int dwAttribute, ref int pvAttribute, int cbAttribute);

        private static int ColorAColorRef(Color color) => color.R | (color.G << 8) | (color.B << 16);

        private void AplicarTemaBarraTitulo()
        {
            try
            {
                int usarOscuro = 1;
                DwmSetWindowAttribute(Handle, DWMWA_USE_IMMERSIVE_DARK_MODE, ref usarOscuro, sizeof(int));

                int colorBorde = ColorAColorRef(ColorBorde);
                DwmSetWindowAttribute(Handle, DWMWA_BORDER_COLOR, ref colorBorde, sizeof(int));

                int backdrop = DWMSBT_MAINWINDOW;
                DwmSetWindowAttribute(Handle, DWMWA_SYSTEMBACKDROP_TYPE, ref backdrop, sizeof(int));
            }
            catch
            {
                // dwmapi.dll no disponible, Mica no soportado (Windows 10 o
                // Windows 11 viejo) o atributo desconocido: se queda con la
                // barra de título clásica, sin romper nada.
            }
        }

        private void TemarBarraDesplazamiento(Control control)
        {
            void Aplicar(object s, EventArgs e) => SetWindowTheme(control.Handle, "DarkMode_Explorer", null);

            if (control.IsHandleCreated)
            {
                Aplicar(null, EventArgs.Empty);
            }

            // Si el handle se recrea (pasa en algunos casos con AutoScroll),
            // hay que volver a aplicarlo o se pierde el tema.
            control.HandleCreated += Aplicar;
        }

        private void AplicarTemaBarrasDesplazamiento()
        {
            TemarBarraDesplazamiento(this);

            foreach (var lista in new Panel[] { ItemsWeapons, ItemsPerks, ItemsGloves, ItemsScripts, ItemsHud })
                TemarBarraDesplazamiento(lista);
        }

        private static void SetPlaceholder(TextBox caja, string texto)
        {
            if (caja == null) return;
            SendMessage(caja.Handle, EM_SETCUEBANNER, IntPtr.Zero, texto);
        }

        // ============================
        // RECARGA (usada al iniciar, al tocar "Recargar" y al cambiar de carpeta)
        // ============================
        private void RecargarTodo()
        {
            var itemsWeapons = PrepararItems(NombresArmas);
            PoblarLista(ItemsWeapons, SearchWeapons, itemsWeapons, AplicarEstadoMod, EliminarMod, ToggleSwitchWeapons);

            var itemsPerks = PrepararItems(NombresPerks);
            PoblarLista(ItemsPerks, SearchPerks, itemsPerks, AplicarEstadoMod, EliminarMod, ToggleSwitchPerks);

            var itemsScripts = PrepararItemsScripts();
            PoblarLista(ItemsScripts, SearchScripts, itemsScripts, AplicarEstadoScript, EliminarScript, ToggleSwitchScripts);

            var itemsGuantes = PrepararItems(NombresGuantes);
            PoblarLista(ItemsGloves, SearchGloves, itemsGuantes, AplicarEstadoMod, EliminarMod, ToggleSwitchGloves);

            var itemsHud = PrepararItemsHud();
            PoblarLista(ItemsHud, SearchHud, itemsHud, AplicarEstadoScript, EliminarScript, ToggleSwitchHud, InstalarHudEmpaquetado);
        }

        private string ResolverCarpetaBaseT6()
        {
            string rutaGuardada = CargarRutaGuardada();
            if (!string.IsNullOrEmpty(rutaGuardada))
            {
                if (Directory.Exists(rutaGuardada))
                {
                    return rutaGuardada;
                }

                MessageBox.Show(
                    Textos.T("MsgCarpetaGuardadaNoExiste"),
                    Textos.T("TituloCarpetaGuardadaNoExiste"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }

            string rutaEstandar = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Plutonium", "storage", "t6"
            );

            if (Directory.Exists(rutaEstandar))
            {
                GuardarRuta(rutaEstandar);
                return rutaEstandar;
            }

            var resultado = MessageBox.Show(
                Textos.T("MsgCarpetaNoEncontrada"),
                Textos.T("TituloCarpetaNoEncontrada"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (resultado != DialogResult.Yes)
            {
                MessageBox.Show(Textos.T("MsgInstalarPlutoniumPrimero"));
                return null;
            }

            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = Textos.T("DescripcionCarpetaT6");

                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    MessageBox.Show(Textos.T("MsgNoSePuedeContinuarSinCarpeta"));
                    return null;
                }

                string rutaElegida = dialog.SelectedPath;
                GuardarRuta(rutaElegida);
                return rutaElegida;
            }
        }

        // ============================
        // BOTONES DE LA BARRA SUPERIOR (Recargar / Cambiar carpeta)
        // ============================
        private void BtnRecargar_Click(object sender, EventArgs e)
        {
            RecargarTodo();
        }

        private static readonly string RutaPlutoniumBootstrapper = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Plutonium", "bin", "plutonium-bootstrapper-win-x64.exe"
        );

        private void BtnIniciarPlutonium_Click(object sender, EventArgs e)
        {
            try
            {
                if (!File.Exists(RutaPlutoniumBootstrapper))
                {
                    MessageBox.Show(
                        Textos.T("MsgPlutoniumNoEncontrado"),
                        Textos.T("TituloPlutoniumNoEncontrado"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = RutaPlutoniumBootstrapper,
                    WorkingDirectory = Path.GetDirectoryName(RutaPlutoniumBootstrapper)
                });
            }
            catch (Exception ex)
            {
                RegistrarError("BtnIniciarPlutonium_Click", ex);
                MessageBox.Show(
                    Textos.F("MsgErrorIniciarPlutonium", ex.Message),
                    Textos.T("TituloErrorIniciarPlutonium"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        private void BtnCambiarCarpeta_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = Textos.T("DescripcionCarpetaT6");

                if (dialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                string nuevaRuta = dialog.SelectedPath;
                GuardarRuta(nuevaRuta);

                carpetaImages = Path.Combine(nuevaRuta, "images");
                carpetaScripts = Path.Combine(nuevaRuta, "scripts", "zm");

                RecargarTodo();
            }
        }

        // Copia uno o más archivos a la carpeta correcta según su extensión:
        // .gsc -> carpeta de scripts, .iwi -> carpeta de images (skins de
        // armas, perks y guantes comparten la misma carpeta de imágenes).
        private void BtnAgregarMod_Click(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Title = Textos.T("TituloSeleccionarArchivos");
                openDialog.Filter =
                    $"{Textos.T("FiltroSkinsScripts")} (*.iwi;*.gsc)|*.iwi;*.gsc|" +
                    $"{Textos.T("FiltroSkins")} (*.iwi)|*.iwi|" +
                    $"{Textos.T("FiltroScripts")} (*.gsc)|*.gsc|" +
                    $"{Textos.T("FiltroTodos")} (*.*)|*.*";
                openDialog.Multiselect = true;

                if (openDialog.ShowDialog() != DialogResult.OK) return;

                int copiados = 0;
                var errores = new List<string>();

                foreach (var rutaOrigen in openDialog.FileNames)
                {
                    string nombreArchivo = Path.GetFileName(rutaOrigen);
                    string extension = Path.GetExtension(rutaOrigen).ToLowerInvariant();

                    string carpetaDestino;
                    if (extension == ".gsc")
                    {
                        carpetaDestino = carpetaScripts;
                    }
                    else if (extension == ".iwi")
                    {
                        carpetaDestino = carpetaImages;
                    }
                    else
                    {
                        errores.Add(Textos.F("ExtensionNoReconocida", nombreArchivo));
                        continue;
                    }

                    try
                    {
                        Directory.CreateDirectory(carpetaDestino);
                        string destino = Path.Combine(carpetaDestino, nombreArchivo);
                        File.Copy(rutaOrigen, destino, true);
                        copiados++;
                    }
                    catch (Exception ex)
                    {
                        RegistrarError($"BtnAgregarMod({nombreArchivo})", ex);
                        errores.Add(nombreArchivo);
                    }
                }

                RecargarTodo();

                if (errores.Count > 0)
                {
                    MessageBox.Show(
                        Textos.F("MsgAgregadosConErrores", copiados, string.Join("\n", errores)),
                        Textos.T("TituloAlgunosNoAgregados"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
                else if (copiados > 0)
                {
                    MessageBox.Show(
                        Textos.F("MsgAgregadosOk", copiados),
                        Textos.T("TituloListo"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
        }

        // ============================
        // EDITOR DE POSICIÓN DEL HUD
        // ============================
        // Abre HudPositionEditorForm apuntando al HudPlayerCustomizable.gsc
        // real, ya sea que esté activo (carpetaScripts) o desactivado
        // (carpetaBackupScripts). Al guardar, el editor llama a RecargarTodo
        // para refrescar la UI (por si el estado activo/inactivo cambió).
        private void BtnEditarPosicionHud_Click(object sender, EventArgs e)
        {
            string rutaEnScripts = Path.Combine(carpetaScripts, ArchivoHudCustomizable);
            string rutaEnBackup = Path.Combine(carpetaBackupScripts, ArchivoHudCustomizable);

            string ruta =
                File.Exists(rutaEnScripts) ? rutaEnScripts :
                File.Exists(rutaEnBackup) ? rutaEnBackup :
                null;

            if (ruta == null)
            {
                MessageBox.Show(
                    Textos.T("MsgArchivoHudNoEncontrado"),
                    Textos.T("TituloArchivoHudNoEncontrado"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            using (var editor = new HudPositionEditorForm(ruta, RecargarTodo))
            {
                editor.ShowDialog(this);
            }
        }

        // ============================
        // ÍCONO DEL HUD (character portrait)
        // ============================
        // A diferencia de las skins de armas/perks, este archivo NO existe por
        // defecto en carpetaImages (el original vive compilado dentro de los
        // assets del juego). Elegir un .iwi acá simplemente lo copia con el
        // nombre fijo NombreArchivoIconoHud, sobrescribiendo lo que hubiera
        // antes. No hay "restablecer": si ya se sobrescribió una vez, no hay
        // forma de recuperar el original desde acá (el código del .gsc nunca
        // se toca, así que siempre sigue apuntando a este mismo material).
        private void BtnCambiarIconoHud_Click(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.Title = Textos.T("TituloSeleccionarIconoHud");
                openDialog.Filter = $"{Textos.T("FiltroIconoHud")} (*.iwi)|*.iwi";
                openDialog.Multiselect = false;

                if (openDialog.ShowDialog() != DialogResult.OK) return;

                try
                {
                    Directory.CreateDirectory(carpetaImages);
                    string destino = Path.Combine(carpetaImages, NombreArchivoIconoHud);
                    File.Copy(openDialog.FileName, destino, true);

                    MessageBox.Show(
                        Textos.T("MsgIconoHudCambiado"),
                        Textos.T("TituloListo"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
                catch (Exception ex)
                {
                    RegistrarError("BtnCambiarIconoHud_Click", ex);
                    MessageBox.Show(
                        Textos.F("MsgErrorCambiarIconoHud", ex.Message),
                        Textos.T("TituloErrorCambiarIconoHud"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
            }
        }

        private string CargarRutaGuardada()
        {
            try
            {
                if (File.Exists(RutaConfig))
                {
                    string contenido = File.ReadAllText(RutaConfig).Trim();
                    return string.IsNullOrWhiteSpace(contenido) ? null : contenido;
                }
            }
            catch (Exception ex)
            {
                RegistrarError("CargarRutaGuardada", ex);
            }
            return null;
        }

        private void GuardarRuta(string ruta)
        {
            try
            {
                string carpetaConfig = Path.GetDirectoryName(RutaConfig);
                Directory.CreateDirectory(carpetaConfig);
                File.WriteAllText(RutaConfig, ruta);
            }
            catch (Exception ex)
            {
                RegistrarError("GuardarRuta", ex);
            }
        }

        // ============================
        // LOG DE ERRORES (para poder diagnosticar reportes del foro)
        // ============================
        private void RegistrarError(string contexto, Exception ex)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(RutaLog));
                File.AppendAllText(
                    RutaLog,
                    $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {contexto}: {ex.Message}{Environment.NewLine}"
                );
            }
            catch
            {
                // Si ni siquiera se puede escribir el log, no hay mucho más para hacer acá.
            }
        }

        private void AgregarCategoria(Panel header, Panel detail, ExpandButton btn, int targetHeight)
        {
            detail.Height = 0;
            detail.Visible = true;

            var timer = new Timer { Interval = 10 };
            var group = new CategoryGroup { Header = header, Detail = detail };

            timer.Tick += (s, e) =>
            {
                int target = btn.Expanded ? targetHeight : 0;
                int current = detail.Height;
                int step = Math.Max(1, Math.Abs(target - current) / 4);

                if (current == target)
                {
                    timer.Stop();
                }
                else if (current < target)
                {
                    detail.Height = Math.Min(target, current + step);
                }
                else
                {
                    detail.Height = Math.Max(target, current - step);
                }
                RepositionAll();
            };

            btn.ExpandedChanged += (s, e) => timer.Start();
            categorias.Add(group);
        }

        // Alto de PanelToolbar (10 arriba + 70 de alto + 10 de separación)
        private const int ContentTop = 120;

        private void RepositionAll()
        {
            int y = ContentTop;
            foreach (var g in categorias)
            {
                g.Header.Location = new Point(10, y);
                y += g.Header.Height + 4;
                g.Detail.Location = new Point(10, y);
                y += g.Detail.Height;
                y += Spacing;
            }

            // Le avisa al form cuánto espacio necesita el contenido para que
            // aparezca el scroll vertical cuando el acordeón crece más allá
            // del alto visible de la ventana.
            this.AutoScrollMinSize = new Size(0, y + 10);
        }

        // ============================
        // DETECCIÓN DE ARCHIVOS
        // ============================
        private List<string> ObtenerArchivosCoincidentes(string carpeta, string token)
        {
            if (!Directory.Exists(carpeta)) return new List<string>();
            return Directory.GetFiles(carpeta)
                .Where(f => Path.GetFileName(f).ToLower().Contains(token.ToLower()))
                .ToList();
        }

        private List<ModItem> PrepararItems(Dictionary<string, string> nombres)
        {
            var lista = new List<ModItem>();

            foreach (var par in nombres)
            {
                if (!TokenArchivo.TryGetValue(par.Key, out string token))
                    continue;

                bool enImages = ObtenerArchivosCoincidentes(carpetaImages, token).Count > 0;
                bool enBackup = ObtenerArchivosCoincidentes(carpetaBackup, token).Count > 0;

                if (enImages || enBackup)
                {
                    lista.Add(new ModItem { Key = par.Key, DisplayName = par.Value, Instalado = enImages });
                }
            }

            return lista;
        }

        // ============================
        // MOVER ARCHIVOS DE SKINS (activar/desactivar de verdad)
        // ============================
        private void AplicarEstadoMod(string key, bool activar)
        {
            if (!TokenArchivo.TryGetValue(key, out string token))
                return;

            try
            {
                if (activar)
                {
                    var enBackup = ObtenerArchivosCoincidentes(carpetaBackup, token);
                    foreach (var archivo in enBackup)
                    {
                        string destino = Path.Combine(carpetaImages, Path.GetFileName(archivo));
                        File.Move(archivo, destino);
                    }
                }
                else
                {
                    var enImages = ObtenerArchivosCoincidentes(carpetaImages, token);
                    foreach (var archivo in enImages)
                    {
                        string destino = Path.Combine(carpetaBackup, Path.GetFileName(archivo));
                        File.Move(archivo, destino);
                    }
                }
            }
            catch (Exception ex)
            {
                RegistrarError($"AplicarEstadoMod({key})", ex);
                MessageBox.Show(
                    Textos.F("MsgErrorMoverMod", ex.Message),
                    Textos.T("TituloErrorAplicarMod"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        // Borra TODOS los archivos asociados al token (arma/perk/guante puede
        // requerir varios archivos), estén activos en carpetaImages o
        // guardados en el backup. Es un borrado definitivo, no un toggle.
        private void EliminarMod(string key)
        {
            if (!TokenArchivo.TryGetValue(key, out string token))
                return;

            try
            {
                var archivos = ObtenerArchivosCoincidentes(carpetaImages, token)
                    .Concat(ObtenerArchivosCoincidentes(carpetaBackup, token));

                foreach (var archivo in archivos)
                {
                    File.Delete(archivo);
                }
            }
            catch (Exception ex)
            {
                RegistrarError($"EliminarMod({key})", ex);
                MessageBox.Show(
                    Textos.F("MsgErrorEliminar", ex.Message),
                    Textos.T("TituloErrorEliminar"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        // ============================
        // SCRIPTS (sin diccionario: se listan directo desde la carpeta)
        // ============================
        private List<ModItem> PrepararItemsScripts()
        {
            var archivos = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (Directory.Exists(carpetaScripts))
                foreach (var f in Directory.GetFiles(carpetaScripts))
                    archivos.Add(Path.GetFileName(f));

            if (Directory.Exists(carpetaBackupScripts))
                foreach (var f in Directory.GetFiles(carpetaBackupScripts))
                    archivos.Add(Path.GetFileName(f));

            archivos.RemoveWhere(a => a.Equals(ArchivoHudCustomizable, StringComparison.OrdinalIgnoreCase));

            var lista = new List<ModItem>();
            foreach (var nombreArchivo in archivos)
            {
                bool enScripts = File.Exists(Path.Combine(carpetaScripts, nombreArchivo));
                lista.Add(new ModItem
                {
                    Key = nombreArchivo,
                    DisplayName = Path.GetFileNameWithoutExtension(nombreArchivo),
                    Instalado = enScripts
                });
            }

            return lista.OrderBy(i => i.DisplayName).ToList();
        }

        // ============================
        // HUD PLAYER
        // ============================
        // A diferencia del resto de las categorías, esta lista SIEMPRE
        // devuelve el item de HudPlayerCustomizable.gsc, exista o no en disco
        // todavía: si no existe en ningún lado (Presente = false), la fila
        // muestra un botón "Instalar" que copia la versión empaquetada con
        // PHDModManager en vez de un toggle + botón de borrado.
        private List<ModItem> PrepararItemsHud()
        {
            bool enScripts = File.Exists(Path.Combine(carpetaScripts, ArchivoHudCustomizable));
            bool enBackup = File.Exists(Path.Combine(carpetaBackupScripts, ArchivoHudCustomizable));

            return new List<ModItem>
            {
                new ModItem
                {
                    Key = ArchivoHudCustomizable,
                    DisplayName = "HUD Player Customizable",
                    Instalado = enScripts,
                    Presente = enScripts || enBackup
                }
            };
        }

        // ============================
        // HUD PLAYER — ARCHIVO EMPAQUETADO (recurso embebido)
        // ============================
        // Extrae el .gsc empaquetado dentro del propio .exe (como recurso
        // embebido) y lo escribe en rutaDestino, sobrescribiendo lo que haya.
        // Se usa tanto para la instalación inicial (cuando el usuario no
        // tiene el archivo) como para "restaurar el original" (cuando sí lo
        // tiene, después de confirmar que quiere perder sus cambios).
        private void ExtraerHudEmpaquetadoA(string rutaDestino)
        {
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            using (var recurso = asm.GetManifestResourceStream(RecursoHudCustomizable))
            {
                if (recurso == null)
                {
                    throw new InvalidOperationException(
                        $"No se encontró el recurso embebido \"{RecursoHudCustomizable}\". " +
                        "Verificá que HudPlayerCustomizable.gsc esté en la carpeta Recursos del " +
                        "proyecto con Build Action = Embedded Resource, y que el namespace por " +
                        "defecto del proyecto coincida.");
                }

                Directory.CreateDirectory(Path.GetDirectoryName(rutaDestino));
                using (var archivoDestino = new FileStream(rutaDestino, FileMode.Create, FileAccess.Write))
                {
                    recurso.CopyTo(archivoDestino);
                }
            }
        }

        // Instala el .gsc empaquetado cuando el usuario TODAVÍA no lo tiene
        // en ningún lado (ni activo ni en backup). Se instala directo en la
        // carpeta de scripts activa, igual que cualquier script agregado a
        // mano con "Añadir skin / script".
        private void InstalarHudEmpaquetado(string key)
        {
            try
            {
                string destino = Path.Combine(carpetaScripts, ArchivoHudCustomizable);
                ExtraerHudEmpaquetadoA(destino);
                RecargarTodo();
            }
            catch (Exception ex)
            {
                RegistrarError("InstalarHudEmpaquetado", ex);
                MessageBox.Show(
                    Textos.F("MsgErrorInstalarHud", ex.Message),
                    Textos.T("TituloErrorInstalarHud"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        // Botón para restaurar la versión original empaquetada aun cuando el
        // usuario YA tiene un HudPlayerCustomizable.gsc propio (por ejemplo,
        // con la posición ya ajustada con el editor). Pide confirmación antes
        // de sobrescribir, porque machacaría esos ajustes sin aviso.
        private void BtnReinstalarHud_Click(object sender, EventArgs e)
        {
            string rutaEnScripts = Path.Combine(carpetaScripts, ArchivoHudCustomizable);
            string rutaEnBackup = Path.Combine(carpetaBackupScripts, ArchivoHudCustomizable);

            // Si ya existe en algún lado, se sobrescribe ESE archivo (para no
            // cambiar el estado activo/inactivo que el usuario ya tenía). Si
            // no existe en ninguno, se instala limpio como activo.
            string destino =
                File.Exists(rutaEnScripts) ? rutaEnScripts :
                File.Exists(rutaEnBackup) ? rutaEnBackup :
                rutaEnScripts;

            bool yaExistia = File.Exists(destino);

            if (yaExistia)
            {
                var confirmacion = MessageBox.Show(
                    Textos.T("MsgConfirmarSobrescribirHud"),
                    Textos.T("TituloConfirmarSobrescribirHud"),
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (confirmacion != DialogResult.Yes) return;
            }

            try
            {
                ExtraerHudEmpaquetadoA(destino);
                RecargarTodo();

                MessageBox.Show(
                    Textos.T("MsgHudReinstalado"),
                    Textos.T("TituloListo"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
            catch (Exception ex)
            {
                RegistrarError("BtnReinstalarHud_Click", ex);
                MessageBox.Show(
                    Textos.F("MsgErrorInstalarHud", ex.Message),
                    Textos.T("TituloErrorInstalarHud"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        private void AplicarEstadoScript(string nombreArchivo, bool activar)
        {
            try
            {
                string origen = activar
                    ? Path.Combine(carpetaBackupScripts, nombreArchivo)
                    : Path.Combine(carpetaScripts, nombreArchivo);

                string destino = activar
                    ? Path.Combine(carpetaScripts, nombreArchivo)
                    : Path.Combine(carpetaBackupScripts, nombreArchivo);

                if (File.Exists(origen))
                {
                    File.Move(origen, destino);
                }
            }
            catch (Exception ex)
            {
                RegistrarError($"AplicarEstadoScript({nombreArchivo})", ex);
                MessageBox.Show(
                    Textos.F("MsgErrorMoverScript", ex.Message),
                    Textos.T("TituloErrorAplicarScript"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        // Borra el script tanto si está activo como si está en backup.
        // A diferencia de EliminarMod, acá se identifica por nombre exacto
        // de archivo, no por token, porque un script normalmente es un
        // único .gsc.
        private void EliminarScript(string nombreArchivo)
        {
            try
            {
                string rutaScripts = Path.Combine(carpetaScripts, nombreArchivo);
                string rutaBackup = Path.Combine(carpetaBackupScripts, nombreArchivo);

                if (File.Exists(rutaScripts)) File.Delete(rutaScripts);
                if (File.Exists(rutaBackup)) File.Delete(rutaBackup);
            }
            catch (Exception ex)
            {
                RegistrarError($"EliminarScript({nombreArchivo})", ex);
                MessageBox.Show(
                    Textos.F("MsgErrorEliminar", ex.Message),
                    Textos.T("TituloErrorEliminar"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        // ============================
        // LISTA SIMPLE (Perks, Gloves, Scripts, HUD) — sin variantes
        // ============================
        private void PoblarLista(Panel contenedor, TextBox buscador, List<ModItem> items, Action<string, bool> aplicarEstado, Action<string> eliminarMod, ToggleSwitch maestro, Action<string> instalarMod = null)
        {
            contenedor.Controls.Clear();

            // El flag de sincronización se comparte entre llamadas (por eso vive en
            // un diccionario aparte y no como variable local): si "Recargar" vuelve a
            // llamar PoblarLista para el mismo contenedor, las filas nuevas y el
            // handler del maestro (que solo se engancha una vez) tienen que usar el
            // mismo flag, si no se pierde la sincronización entre ambos.
            if (!_flagsSincronizacion.TryGetValue(contenedor, out bool[] sincronizandoMaestro))
            {
                sincronizandoMaestro = new bool[1];
                _flagsSincronizacion[contenedor] = sincronizandoMaestro;
            }

            void ActualizarMaestro()
            {
                bool algunoActivo = false;
                foreach (Control fila in contenedor.Controls)
                {
                    foreach (Control child in fila.Controls)
                    {
                        if (child is ToggleSwitch ts && ts.Checked)
                        {
                            algunoActivo = true;
                        }
                    }
                }

                sincronizandoMaestro[0] = true;
                maestro.Checked = algunoActivo;
                sincronizandoMaestro[0] = false;
            }

            int y = 0;
            foreach (var item in items)
            {
                var row = new Panel { Size = new Size(contenedor.Width - 20, 28), Location = new Point(0, y), Tag = item, BackColor = ColorControlInterno };

                var lbl = new Label { Text = item.DisplayName, AutoSize = true, Location = new Point(4, 6), ForeColor = ColorTexto, BackColor = Color.Transparent };
                row.Controls.Add(lbl);

                if (item.Presente)
                {
                    var toggle = new ToggleSwitch { Size = new Size(40, 20), Location = new Point(row.Width - 50, 4), Checked = item.Instalado };

                    var btnEliminar = new Button
                    {
                        Text = "X",
                        Size = new Size(24, 20),
                        Location = new Point(row.Width - 80, 4),
                        FlatStyle = FlatStyle.Flat,
                        ForeColor = ColorEliminar,
                        BackColor = ColorControlInterno
                    };
                    btnEliminar.FlatAppearance.BorderColor = ColorBorde;
                    btnEliminar.FlatAppearance.MouseOverBackColor = ColorBotonHover;

                    btnEliminar.Click += (s, e) =>
                    {
                        var confirmacion = MessageBox.Show(
                            Textos.F("MsgConfirmarEliminar", item.DisplayName),
                            Textos.T("TituloConfirmarEliminar"),
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning
                        );

                        if (confirmacion == DialogResult.Yes)
                        {
                            eliminarMod(item.Key);
                            RecargarTodo();
                        }
                    };

                    toggle.CheckedChanged += (s, e) =>
                    {
                        aplicarEstado(item.Key, toggle.Checked);
                        if (!sincronizandoMaestro[0])
                        {
                            ActualizarMaestro();
                        }
                    };

                    row.Controls.Add(btnEliminar);
                    row.Controls.Add(toggle);
                }
                else
                {
                    // Todavía no está instalado en ningún lado (caso actual:
                    // HUD Player recién empaquetado, antes de que el usuario
                    // lo instale). No hay nada que togglear ni borrar: se
                    // ofrece un botón para instalarlo.
                    var btnInstalar = new Button
                    {
                        Text = Textos.T("BtnInstalarHud"),
                        Size = new Size(90, 20),
                        Location = new Point(row.Width - 94, 4),
                        FlatStyle = FlatStyle.Flat,
                        BackColor = ColorBotonFondo,
                        ForeColor = ColorTexto
                    };
                    btnInstalar.FlatAppearance.BorderColor = ColorBorde;
                    btnInstalar.FlatAppearance.MouseOverBackColor = ColorBotonHover;
                    btnInstalar.FlatAppearance.MouseDownBackColor = ColorAcento;

                    btnInstalar.Click += (s, e) =>
                    {
                        instalarMod?.Invoke(item.Key);
                    };

                    row.Controls.Add(btnInstalar);
                }

                contenedor.Controls.Add(row);
                y += 32;
            }

            // El buscador y el toggle maestro solo se enganchan la primera vez que
            // se puebla este contenedor; si no, cada "Recargar" sumaría una
            // suscripción más y el filtro/sincronización se ejecutaría N veces.
            if (!_listasEnganchadas.Contains(contenedor))
            {
                _listasEnganchadas.Add(contenedor);

                buscador.TextChanged += (s, e) => FiltrarLista(contenedor, buscador.Text);

                maestro.CheckedChanged += (s, e) =>
                {
                    if (sincronizandoMaestro[0]) return;

                    sincronizandoMaestro[0] = true;
                    foreach (Control row in contenedor.Controls)
                    {
                        foreach (Control child in row.Controls)
                        {
                            if (child is ToggleSwitch ts && ts.Checked != maestro.Checked)
                            {
                                ts.Checked = maestro.Checked;
                            }
                        }
                    }
                    sincronizandoMaestro[0] = false;
                };
            }

            ActualizarMaestro();
        }

        private void FiltrarLista(Panel contenedor, string texto)
        {
            int y = 0;
            foreach (Control row in contenedor.Controls)
            {
                var item = (ModItem)row.Tag;
                bool coincide = item.DisplayName.ToLower().Contains(texto.ToLower());
                row.Visible = coincide;
                if (coincide)
                {
                    row.Location = new Point(0, y);
                    y += 32;
                }
            }
        }

    }
}