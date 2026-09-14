using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace PHDModManager
{
    // Editor visual para ajustar la posición (x, y) de DOS elementos del HUD
    // que usa HudPlayerCustomizable.gsc:
    //   - El ícono del personaje (hudElem "character_hud").
    //   - El texto fijo que se muestra en pantalla (fontstring "uwu_text",
    //     cuyo contenido también se puede editar acá).
    // Estos dos elementos son completamente independientes en el script (no
    // hay setParent entre ellos), así que cada uno tiene su propio marcador
    // en el lienzo y su propio par de coordenadas.
    //
    // A propósito, esta ventana NO tiene un .Designer.cs aparte: todo el
    // layout se arma acá mismo en código. Es un formulario nuevo e
    // independiente de MainForm, así que esto no afecta al diseñador de
    // MainForm para nada, pero se evita sumar otro archivo generado más
    // dado el historial de corrupción del diseñador en este proyecto.
    //
    // CONVENCIÓN DE COORDENADAS (calibrada a partir de mediciones reales en
    // juego — ver historial de pruebas):
    //   - Los dos ejes (x e y) usan la MISMA escala física, y esa escala se
    //     calcula siempre a partir del ALTO real de pantalla (altoReal / 480).
    //     El motor NO estira el HUD horizontalmente en pantallas panorámicas:
    //     en una pantalla ancha simplemente hay más espacio real disponible
    //     del lado derecho (para elementos con horzalign="left", el borde
    //     IZQUIERDO sigue estando en x = 0; el que se corre es el borde
    //     DERECHO real, mucho más lejos que el x = 640 que se asumía antes).
    //   - vertalign = "bottom" / punto "BOTTOM_LEFT" -> y se mide desde el
    //     borde INFERIOR. y = 0 es el borde; NEGATIVO sube el elemento desde
    //     abajo (como y = -103 / y = -105 originales); positivo lo bajaría
    //     por debajo del borde (fuera de pantalla).
    //   Se asume la MISMA convención para el ícono y el texto porque ambos
    //   usan un anclaje "abajo-izquierda" en el script.
    //
    //   OJO: esto NO resuelve el otro problema reportado (posiciones que
    //   "desaparecen" en pantalla, ej. y = -391) — ese sigue en
    //   investigación aparte y no se tocó nada relacionado a propósito.
    public class HudPositionEditorForm : Form
    {
        // --- Geometría del "lienzo" ---
        // AltoVirtual (480) es la referencia vertical fija del motor: la
        // escala de AMBOS ejes sale siempre de acá. El "ancho real" de la
        // pantalla, en cambio, depende de la resolución del juego (ver
        // _numAnchoJuego / _numAltoJuego / _anchoVirtualDerecho más abajo).
        private const int AltoVirtual = 480;

        // Margen extra alrededor del rectángulo de la pantalla, para poder
        // representar (y arrastrar hacia) valores fuera de pantalla, como
        // el x = -47 original.
        private const int MargenVirtual = 80;

        private const int AltoVirtualTotal = AltoVirtual + MargenVirtual * 2;

        // Tamaño del panel de dibujo en píxeles reales de la ventana.
        // YA NO son constantes fijas: se calculan en el constructor según
        // el espacio disponible en la pantalla del usuario (ver
        // CalcularTamanioLienzo), para que la ventana entre completa incluso
        // en laptops con resolución chica (1366x768 o menos). En un monitor
        // grande, siguen llegando al tamaño "ideal" de antes (680x420).
        private readonly int AnchoPanelPx;
        private readonly int AltoPanelPx;

        // Límites del lienzo: nunca más chico que esto (para que siga siendo
        // usable) ni más grande que esto (tamaño "ideal" original).
        private const int AltoPanelMinimo = 200;
        private const int AltoPanelMaximo = 420;
        private const float AspectoLienzo = 16f / 9f;

        // Una sola escala para los dos ejes (px del panel por unidad GSC),
        // calculada siempre a partir del alto — así el marcador se mueve
        // igual de rápido en x que en y, igual que en el juego real.
        // (Ya no es "static" porque AltoPanelPx ahora es de instancia.)
        private float Escala => (float)AltoPanelPx / AltoVirtualTotal;

        // Borde derecho real de la pantalla, en unidades GSC, calculado a
        // partir de la resolución del juego que indique el usuario (ver
        // ActualizarAnchoVirtualDerecho). Arranca asumiendo 1920x1080.
        private int _anchoVirtualDerecho;

        private const int RadioMarcador = 10;

        // Colores para distinguir los dos marcadores en el lienzo.
        private static readonly Color ColorIcono = Color.DeepSkyBlue;
        private static readonly Color ColorTexto = Color.Orange;

        // Nombre del archivo de imagen de referencia (captura de pantalla del
        // juego). Se busca al lado del .exe. Si no está, el lienzo se dibuja
        // con el fondo oscuro de siempre, sin romper nada.
        private const string NombreArchivoFondo = "FondoHud.png";

        // Valores originales conocidos y "buenos" (los que ya sabemos que funcionan).
        private const int XIconoOriginal = -47;
        private const int YIconoOriginal = -103;
        private const int XTextoOriginal = -10;
        private const int YTextoOriginal = -105;
        private const string TextoOriginalPorDefecto = "@ApthSlayer115";

        // Posición actual de cada marcador, en coordenadas GSC (las que van al archivo).
        private int _xIcono;
        private int _yIcono;
        private int _xTexto;
        private int _yTexto;
        private string _textoNombre = TextoOriginalPorDefecto;

        // Cuál de los dos marcadores está "activo" (el que mueven el mouse
        // y los botones de esquina/restablecer).
        private bool _marcadorActivoEsIcono = true;
        private bool _arrastrando;

        private Image _imagenFondo;

        private readonly string _rutaGsc;
        private readonly Action _onGuardado;

        // --- Controles ---
        private Label _lblAviso;
        private RadioButton _radioIcono;
        private RadioButton _radioTexto;
        private Panel _panelLienzo;
        private Label _lblPosicionActual;
        private Button _btnEsquinaAbajoIzquierda;
        private Button _btnEsquinaAbajoDerecha;
        private Button _btnEsquinaArribaIzquierda;
        private Button _btnEsquinaArribaDerecha;
        private Button _btnRestablecer;
        private Label _lblResolucion;
        private NumericUpDown _numAnchoJuego;
        private NumericUpDown _numAltoJuego;
        private Label _lblTextoNombre;
        private TextBox _txtNombre;
        private Button _btnGuardar;
        private Button _btnCancelar;

        // rutaArchivoGsc: ruta completa al HudPlayerCustomizable.gsc que se va a
        // leer/editar (ya sea en la carpeta de scripts activa o en el backup).
        // onGuardado: callback que se invoca después de guardar con éxito
        // (en MainForm, normalmente RecargarTodo) para refrescar la UI principal.
        public HudPositionEditorForm(string rutaArchivoGsc, Action onGuardado)
        {
            _rutaGsc = rutaArchivoGsc;
            _onGuardado = onGuardado;

            _anchoVirtualDerecho = CalcularAnchoVirtualDerecho(1920, 1080);

            CalcularTamanioLienzo(out AltoPanelPx, out AnchoPanelPx);

            ConstruirUi();
            AsegurarQueEntraEnPantalla();
            CargarImagenFondo();
            CargarPosicionDesdeArchivo();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _imagenFondo?.Dispose();
            }
            base.Dispose(disposing);
        }

        // ============================
        // TAMAÑO DEL LIENZO SEGÚN LA PANTALLA DISPONIBLE
        // ============================
        // El resto de los controles (avisos, botones, campos) ocupan un
        // alto casi fijo (~400px) sin importar el tamaño del lienzo. Esta
        // cuenta reserva ese espacio y reparte lo que queda de la pantalla
        // entre el lienzo, con un piso y un techo razonables.
        private const int AltoReservadoControlesFijos = 420;
        private const int MargenSeguridadPantalla = 60;

        private static void CalcularTamanioLienzo(out int altoPanel, out int anchoPanel)
        {
            var areaTrabajo = Screen.FromPoint(Cursor.Position).WorkingArea;

            int altoDisponibleParaLienzo = areaTrabajo.Height - AltoReservadoControlesFijos - MargenSeguridadPantalla;
            altoPanel = Math.Max(AltoPanelMinimo, Math.Min(AltoPanelMaximo, altoDisponibleParaLienzo));

            anchoPanel = (int)Math.Round(altoPanel * AspectoLienzo);

            int anchoMaximoDisponible = areaTrabajo.Width - 100;
            if (anchoPanel > anchoMaximoDisponible)
            {
                anchoPanel = Math.Max(280, anchoMaximoDisponible);
                // Si tuvimos que angostar por ancho, recalculamos el alto
                // en base a ese ancho para no dejar el lienzo deformado.
                altoPanel = (int)Math.Round(anchoPanel / AspectoLienzo);
            }
        }

        // Red de seguridad final: si aun con el cálculo de arriba la ventana
        // terminó más alta que la pantalla (por ejemplo, con una resolución
        // de juego rarísima que agranda mucho el texto de algún label), se
        // activa scroll en vez de dejarla cortada fuera de la pantalla.
        private void AsegurarQueEntraEnPantalla()
        {
            var areaTrabajo = Screen.FromPoint(Cursor.Position).WorkingArea;
            int altoMaximoVentana = areaTrabajo.Height - 20;

            if (this.Height > altoMaximoVentana)
            {
                this.AutoScroll = true;
                this.MaximumSize = new Size(this.Width + 40, altoMaximoVentana);
                this.Height = altoMaximoVentana;
            }
        }

        // ============================
        // CONSTRUCCIÓN DE LA UI (sin Designer.cs)
        // ============================
        private void ConstruirUi()
        {
            this.Text = Textos.T("TituloEditorHud");
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            // CenterScreen en vez de CenterParent: si MainForm está cerca del
            // borde de una pantalla chica, centrar sobre él podía empujar
            // este diálogo fuera de la pantalla. Centrado en la pantalla
            // completa es más seguro para una ventana de este tamaño.
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Segoe UI", 9F);

            _lblAviso = new Label
            {
                Text = Textos.T("LblAvisoPosicionAproximada"),
                ForeColor = Color.DarkOrange,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(10, 10),
                Size = new Size(AnchoPanelPx + 20, 40)
            };

            // Selector de marcador activo. El color del texto coincide con
            // el color del marcador correspondiente en el lienzo, para que
            // sea obvio cuál es cuál sin necesitar una leyenda aparte.
            _radioIcono = new RadioButton
            {
                Text = Textos.T("EtiquetaMarcadorIcono"),
                ForeColor = ColorIcono,
                AutoSize = true,
                Checked = true,
                Location = new Point(20, _lblAviso.Bottom + 6)
            };

            _radioTexto = new RadioButton
            {
                Text = Textos.T("EtiquetaMarcadorTexto"),
                ForeColor = ColorTexto,
                AutoSize = true,
                Location = new Point(_radioIcono.Right + 30, _radioIcono.Top)
            };

            _radioIcono.CheckedChanged += (s, e) =>
            {
                if (_radioIcono.Checked)
                {
                    _marcadorActivoEsIcono = true;
                    ActualizarEtiquetaPosicion();
                    _panelLienzo.Invalidate();
                }
            };
            _radioTexto.CheckedChanged += (s, e) =>
            {
                if (_radioTexto.Checked)
                {
                    _marcadorActivoEsIcono = false;
                    ActualizarEtiquetaPosicion();
                    _panelLienzo.Invalidate();
                }
            };

            _panelLienzo = new Panel
            {
                Location = new Point(20, _radioIcono.Bottom + 10),
                Size = new Size(AnchoPanelPx, AltoPanelPx),
                BackColor = Color.FromArgb(45, 45, 48),
                BorderStyle = BorderStyle.FixedSingle
            };
            _panelLienzo.Paint += PanelLienzo_Paint;
            _panelLienzo.MouseDown += PanelLienzo_MouseDown;
            _panelLienzo.MouseMove += PanelLienzo_MouseMove;
            _panelLienzo.MouseUp += PanelLienzo_MouseUp;

            _lblPosicionActual = new Label
            {
                AutoSize = true,
                Location = new Point(20, _panelLienzo.Bottom + 8),
                Font = new Font("Consolas", 9.5F, FontStyle.Bold)
            };

            int anchoBotonEsquina = (AnchoPanelPx - 10) / 2;

            _btnEsquinaArribaIzquierda = new Button
            {
                Text = Textos.T("BtnEsquinaArribaIzquierda"),
                Location = new Point(20, _lblPosicionActual.Bottom + 10),
                Size = new Size(anchoBotonEsquina, 26),
                FlatStyle = FlatStyle.Flat
            };
            _btnEsquinaArribaIzquierda.Click += (s, e) => EstablecerPosicionActiva(0, -AltoVirtual);

            _btnEsquinaArribaDerecha = new Button
            {
                Text = Textos.T("BtnEsquinaArribaDerecha"),
                Location = new Point(_btnEsquinaArribaIzquierda.Right + 10, _btnEsquinaArribaIzquierda.Top),
                Size = new Size(anchoBotonEsquina, 26),
                FlatStyle = FlatStyle.Flat
            };
            _btnEsquinaArribaDerecha.Click += (s, e) => EstablecerPosicionActiva(_anchoVirtualDerecho, -AltoVirtual);

            _btnEsquinaAbajoIzquierda = new Button
            {
                Text = Textos.T("BtnEsquinaAbajoIzquierda"),
                Location = new Point(20, _btnEsquinaArribaIzquierda.Bottom + 6),
                Size = new Size(anchoBotonEsquina, 26),
                FlatStyle = FlatStyle.Flat
            };
            _btnEsquinaAbajoIzquierda.Click += (s, e) => EstablecerPosicionActiva(0, 0);

            _btnEsquinaAbajoDerecha = new Button
            {
                Text = Textos.T("BtnEsquinaAbajoDerecha"),
                Location = new Point(_btnEsquinaAbajoIzquierda.Right + 10, _btnEsquinaAbajoIzquierda.Top),
                Size = new Size(anchoBotonEsquina, 26),
                FlatStyle = FlatStyle.Flat
            };
            _btnEsquinaAbajoDerecha.Click += (s, e) => EstablecerPosicionActiva(_anchoVirtualDerecho, 0);

            _btnRestablecer = new Button
            {
                Text = Textos.T("BtnRestablecerOriginal"),
                Location = new Point(20, _btnEsquinaAbajoIzquierda.Bottom + 12),
                Size = new Size(AnchoPanelPx, 28),
                FlatStyle = FlatStyle.Flat
            };
            _btnRestablecer.Click += (s, e) => EstablecerPosicionActiva(
                _marcadorActivoEsIcono ? XIconoOriginal : XTextoOriginal,
                _marcadorActivoEsIcono ? YIconoOriginal : YTextoOriginal
            );

            // Resolución del juego: determina dónde cae el borde derecho
            // REAL de la pantalla (los botones "Arriba/Abajo-Derecha" y el
            // rectángulo del lienzo se ajustan según esto). El borde
            // izquierdo y el vertical no dependen de esto.
            _lblResolucion = new Label
            {
                Text = Textos.T("LblResolucionJuego"),
                AutoSize = true,
                Location = new Point(20, _btnRestablecer.Bottom + 14)
            };

            _numAnchoJuego = new NumericUpDown
            {
                Minimum = 640,
                Maximum = 7680,
                Value = 1920,
                Increment = 10,
                Location = new Point(20, _lblResolucion.Bottom + 4),
                Size = new Size(90, 24)
            };

            var lblPorX = new Label
            {
                Text = "x",
                AutoSize = true,
                Location = new Point(_numAnchoJuego.Right + 6, _numAnchoJuego.Top + 3)
            };

            _numAltoJuego = new NumericUpDown
            {
                Minimum = 480,
                Maximum = 4320,
                Value = 1080,
                Increment = 10,
                Location = new Point(lblPorX.Right + 6, _numAnchoJuego.Top),
                Size = new Size(90, 24)
            };

            _numAnchoJuego.ValueChanged += (s, e) => ActualizarAnchoVirtualDerecho();
            _numAltoJuego.ValueChanged += (s, e) => ActualizarAnchoVirtualDerecho();

            _lblTextoNombre = new Label
            {
                Text = Textos.T("LblTextoNombre"),
                AutoSize = true,
                Location = new Point(20, _numAnchoJuego.Bottom + 14)
            };

            _txtNombre = new TextBox
            {
                Location = new Point(20, _lblTextoNombre.Bottom + 4),
                Size = new Size(AnchoPanelPx, 24)
            };

            _btnGuardar = new Button
            {
                Text = Textos.T("BtnGuardarPosicion"),
                Location = new Point(20, _txtNombre.Bottom + 14),
                Size = new Size((AnchoPanelPx - 10) / 2, 30),
                FlatStyle = FlatStyle.Flat
            };
            _btnGuardar.Click += BtnGuardar_Click;

            _btnCancelar = new Button
            {
                Text = Textos.T("BtnCancelar"),
                Location = new Point(_btnGuardar.Right + 10, _btnGuardar.Top),
                Size = new Size((AnchoPanelPx - 10) / 2, 30),
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };

            this.Controls.Add(_lblAviso);
            this.Controls.Add(_radioIcono);
            this.Controls.Add(_radioTexto);
            this.Controls.Add(_panelLienzo);
            this.Controls.Add(_lblPosicionActual);
            this.Controls.Add(_btnEsquinaArribaIzquierda);
            this.Controls.Add(_btnEsquinaArribaDerecha);
            this.Controls.Add(_btnEsquinaAbajoIzquierda);
            this.Controls.Add(_btnEsquinaAbajoDerecha);
            this.Controls.Add(_btnRestablecer);
            this.Controls.Add(_lblResolucion);
            this.Controls.Add(_numAnchoJuego);
            this.Controls.Add(lblPorX);
            this.Controls.Add(_numAltoJuego);
            this.Controls.Add(_lblTextoNombre);
            this.Controls.Add(_txtNombre);
            this.Controls.Add(_btnGuardar);
            this.Controls.Add(_btnCancelar);

            this.CancelButton = _btnCancelar;

            // Alto calculado en base al último control, así no hay que
            // recalcular a mano cada vez que se agrega una fila nueva.
            this.ClientSize = new Size(AnchoPanelPx + 40, _btnCancelar.Bottom + 20);
        }

        // ============================
        // BORDE DERECHO REAL (según resolución del juego)
        // ============================
        // El motor no estira el HUD horizontalmente: la escala real (px de
        // pantalla por unidad GSC) es siempre altoReal / 480, en los DOS
        // ejes. Entonces el borde derecho real, en unidades GSC, es
        // anchoReal / esaMismaEscala = 480 * (anchoReal / altoReal).
        // Para una pantalla 4:3 clásica (1.333) da exactamente 640 (el
        // valor que se asumía fijo antes); para 16:9 (1.778) da ~853.
        private static int CalcularAnchoVirtualDerecho(int anchoReal, int altoReal)
        {
            float aspecto = (float)anchoReal / altoReal;
            return (int)Math.Round(AltoVirtual * aspecto);
        }

        private void ActualizarAnchoVirtualDerecho()
        {
            _anchoVirtualDerecho = CalcularAnchoVirtualDerecho((int)_numAnchoJuego.Value, (int)_numAltoJuego.Value);
            _panelLienzo.Invalidate();
        }

        // ============================
        // IMAGEN DE FONDO (referencia del juego)
        // ============================
        // Busca "FondoHud.png" al lado del .exe. Si no existe, no pasa nada:
        // el lienzo se ve como antes (fondo oscuro liso). Para agregarla:
        //   1. Copiá FondoHud.png a la carpeta del proyecto (en la raíz).
        //   2. En el Explorador de soluciones: Agregar > Elemento existente > FondoHud.png.
        //   3. Click derecho sobre el archivo agregado > Propiedades >
        //      "Copiar en el directorio de salida" = "Copiar si es más reciente".
        private void CargarImagenFondo()
        {
            try
            {
                string ruta = Path.Combine(Application.StartupPath, NombreArchivoFondo);
                if (File.Exists(ruta))
                {
                    // Se carga a un Bitmap propio (en vez de usar Image.FromFile
                    // directo) para no dejar el archivo bloqueado en disco.
                    using (var original = Image.FromFile(ruta))
                    {
                        _imagenFondo = new Bitmap(original);
                    }
                }
            }
            catch
            {
                _imagenFondo = null;
            }
        }

        // ============================
        // LECTURA DEL ARCHIVO
        // ============================
        private void CargarPosicionDesdeArchivo()
        {
            try
            {
                if (!File.Exists(_rutaGsc))
                {
                    MessageBox.Show(
                        Textos.T("MsgArchivoHudNoEncontrado"),
                        Textos.T("TituloArchivoHudNoEncontrado"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    _xIcono = XIconoOriginal;
                    _yIcono = YIconoOriginal;
                    _xTexto = XTextoOriginal;
                    _yTexto = YTextoOriginal;
                    _textoNombre = TextoOriginalPorDefecto;
                }
                else
                {
                    string contenido = File.ReadAllText(_rutaGsc);

                    var matchX = Regex.Match(contenido, @"character_hud\.x\s*=\s*(-?\d+)\s*;");
                    var matchY = Regex.Match(contenido, @"character_hud\.y\s*=\s*(-?\d+)\s*;");
                    _xIcono = matchX.Success ? int.Parse(matchX.Groups[1].Value) : XIconoOriginal;
                    _yIcono = matchY.Success ? int.Parse(matchY.Groups[1].Value) : YIconoOriginal;

                    var matchPosTexto = Regex.Match(
                        contenido,
                        @"uwu_text\s*setpoint\(\s*""[^""]*""\s*,\s*""[^""]*""\s*,\s*(-?\d+)\s*,\s*(-?\d+)\s*\)\s*;"
                    );
                    _xTexto = matchPosTexto.Success ? int.Parse(matchPosTexto.Groups[1].Value) : XTextoOriginal;
                    _yTexto = matchPosTexto.Success ? int.Parse(matchPosTexto.Groups[2].Value) : YTextoOriginal;

                    var matchTexto = Regex.Match(contenido, @"uwu_text\s*settext\(\s*""([^""]*)""\s*\)\s*;");
                    _textoNombre = matchTexto.Success ? matchTexto.Groups[1].Value : TextoOriginalPorDefecto;
                }
            }
            catch
            {
                // Si algo falla leyendo, arrancamos igual con los valores
                // originales conocidos en vez de dejar la ventana en un
                // estado raro (sin marcador, con NaN, etc).
                _xIcono = XIconoOriginal;
                _yIcono = YIconoOriginal;
                _xTexto = XTextoOriginal;
                _yTexto = YTextoOriginal;
                _textoNombre = TextoOriginalPorDefecto;
            }

            _txtNombre.Text = _textoNombre;
            ActualizarEtiquetaPosicion();
            _panelLienzo.Invalidate();
        }

        // ============================
        // GUARDADO
        // ============================
        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            string nuevoTexto = _txtNombre.Text ?? string.Empty;
            if (nuevoTexto.Contains("\""))
            {
                MessageBox.Show(
                    Textos.T("MsgTextoConComillas"),
                    Textos.T("TituloTextoInvalido"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            try
            {
                if (!File.Exists(_rutaGsc))
                {
                    MessageBox.Show(
                        Textos.T("MsgArchivoHudNoEncontrado"),
                        Textos.T("TituloArchivoHudNoEncontrado"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                    return;
                }

                _textoNombre = nuevoTexto;
                string contenido = File.ReadAllText(_rutaGsc);

                // --- Ícono (character_hud): 4 ocurrencias esperadas, una por personaje ---
                string patronX = @"(character_hud\.x\s*=\s*)-?\d+(\s*;)";
                string patronY = @"(character_hud\.y\s*=\s*)-?\d+(\s*;)";

                int reemplazosX = Regex.Matches(contenido, patronX).Count;
                int reemplazosY = Regex.Matches(contenido, patronY).Count;

                // IMPORTANTE: se usa un MatchEvaluator (lambda), NO un string,
                // como reemplazo. Si se pasa un string, .NET interpreta "$"
                // seguido de dígitos como referencia a grupo capturado — y un
                // valor positivo pegado a "$1"/"$2"/"$3" (ej. "$1" + "10")
                // se lee como "$110", un grupo inexistente, y se pierde todo
                // el texto del grupo 1. Con un MatchEvaluator el texto se
                // inserta literal, sin ninguna interpretación de "$".
                contenido = Regex.Replace(contenido, patronX,
                    m => m.Groups[1].Value + _xIcono + m.Groups[2].Value);
                contenido = Regex.Replace(contenido, patronY,
                    m => m.Groups[1].Value + _yIcono + m.Groups[2].Value);

                // --- Nombre en pantalla (uwu_text): posición, 1 ocurrencia esperada ---
                string patronPosTexto = @"(uwu_text\s*setpoint\(\s*""[^""]*""\s*,\s*""[^""]*""\s*,\s*)-?\d+(\s*,\s*)-?\d+(\s*\)\s*;)";
                int reemplazosPosTexto = Regex.Matches(contenido, patronPosTexto).Count;
                contenido = Regex.Replace(contenido, patronPosTexto,
                    m => m.Groups[1].Value + _xTexto + m.Groups[2].Value + _yTexto + m.Groups[3].Value);

                // --- Nombre en pantalla (uwu_text): contenido del texto, 1 ocurrencia esperada ---
                // También con MatchEvaluator: si el texto que escribe el usuario
                // contuviera un "$" seguido de dígitos, un reemplazo por string
                // tendría el mismo problema.
                string patronTextoContenido = @"(uwu_text\s*settext\(\s*"")[^""]*(""\s*\)\s*;)";
                int reemplazosTexto = Regex.Matches(contenido, patronTextoContenido).Count;
                contenido = Regex.Replace(contenido, patronTextoContenido,
                    m => m.Groups[1].Value + _textoNombre + m.Groups[2].Value);

                File.WriteAllText(_rutaGsc, contenido);

                // Avisos (no bloqueantes) si no se encontraron las ocurrencias
                // esperadas, por si el archivo fue editado a mano y ya no
                // tiene la forma esperada. El archivo igual se guarda con
                // los reemplazos que sí se pudieron hacer.
                if (reemplazosX != 4 || reemplazosY != 4)
                {
                    MessageBox.Show(
                        Textos.F("MsgAdvertenciaCantidadReemplazos", reemplazosX, reemplazosY),
                        Textos.T("TituloAdvertenciaCantidadReemplazos"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }

                if (reemplazosPosTexto != 1 || reemplazosTexto != 1)
                {
                    MessageBox.Show(
                        Textos.F("MsgAdvertenciaReemplazoTexto", reemplazosPosTexto, reemplazosTexto),
                        Textos.T("TituloAdvertenciaReemplazoTexto"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }

                _onGuardado?.Invoke();

                MessageBox.Show(
                    Textos.T("MsgPosicionGuardada"),
                    Textos.T("TituloPosicionGuardada"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    Textos.F("MsgErrorGuardarPosicion", ex.Message),
                    Textos.T("TituloErrorGuardarPosicion"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
            }
        }

        // ============================
        // CONVERSIONES ENTRE PÍXELES DEL PANEL Y COORDENADAS GSC
        // ============================

        // Convierte una posición del mouse dentro de _panelLienzo (píxeles,
        // origen arriba-izquierda) a coordenadas x/y del juego. Ver el
        // comentario de convención al principio de la clase.
        private void ConvertirPanelAVirtual(int panelX, int panelY, out int x, out int y)
        {
            float margenPx = MargenVirtual * Escala;
            float virtualXDesdeIzquierda = (panelX - margenPx) / Escala;

            float virtualYDesdeArriba = (panelY - margenPx) / Escala;
            float virtualYDesdeAbajo = AltoVirtual - virtualYDesdeArriba;

            int xCalculado = (int)Math.Round(virtualXDesdeIzquierda);
            int yCalculado = (int)Math.Round(-virtualYDesdeAbajo);

            // Clamp: evita valores absurdos si el arrastre sigue de largo
            // fuera del panel (el mouse puede reportar coordenadas negativas
            // o mayores al tamaño del control mientras dura el arrastre).
            // El límite derecho ahora es el borde real calculado según la
            // resolución del juego, no un 640 fijo.
            x = Math.Max(-MargenVirtual, Math.Min(_anchoVirtualDerecho + MargenVirtual, xCalculado));
            y = Math.Max(-(AltoVirtual + MargenVirtual), Math.Min(MargenVirtual, yCalculado));
        }

        // Conversión inversa: de coordenadas x/y del juego a un punto en
        // píxeles dentro de _panelLienzo, para dibujar el marcador.
        private PointF ConvertirVirtualAPanel(int x, int y)
        {
            float virtualYDesdeAbajo = -y;
            float virtualYDesdeArriba = AltoVirtual - virtualYDesdeAbajo;

            float panelX = (x + MargenVirtual) * Escala;
            float panelY = (virtualYDesdeArriba + MargenVirtual) * Escala;

            return new PointF(panelX, panelY);
        }

        private void EstablecerPosicionActiva(int x, int y)
        {
            if (_marcadorActivoEsIcono)
            {
                _xIcono = x;
                _yIcono = y;
            }
            else
            {
                _xTexto = x;
                _yTexto = y;
            }
            ActualizarEtiquetaPosicion();
            _panelLienzo.Invalidate();
        }

        private void ActualizarEtiquetaPosicion()
        {
            string etiqueta = _marcadorActivoEsIcono
                ? Textos.T("EtiquetaMarcadorIcono")
                : Textos.T("EtiquetaMarcadorTexto");
            int x = _marcadorActivoEsIcono ? _xIcono : _xTexto;
            int y = _marcadorActivoEsIcono ? _yIcono : _yTexto;

            _lblPosicionActual.Text = Textos.F("LblPosicionActual", etiqueta, x, y);
        }

        // ============================
        // DIBUJO DEL LIENZO
        // ============================
        private void PanelLienzo_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Rectángulo que representa la pantalla real dentro del lienzo
            // total (que incluye el margen de "fuera de pantalla"). El ancho
            // ahora refleja el borde derecho real según la resolución
            // elegida (antes era un 640 fijo que no correspondía a
            // pantallas panorámicas).
            float margenPx = MargenVirtual * Escala;
            float anchoPantallaPx = _anchoVirtualDerecho * Escala;
            float altoPantallaPx = AltoVirtual * Escala;

            var rectPantalla = new RectangleF(margenPx, margenPx, anchoPantallaPx, altoPantallaPx);

            // Fondo oscuro de base SIEMPRE (incluso con imagen cargada),
            // para rellenar las barras que quedan si la imagen no coincide
            // exactamente con la proporción de rectPantalla.
            using (var brushPantalla = new SolidBrush(Color.FromArgb(15, 15, 15)))
            {
                g.FillRectangle(brushPantalla, rectPantalla);
            }

            if (_imagenFondo != null)
            {
                // Se dibuja "contain" (mantiene la proporción real de la
                // imagen) en vez de estirarla a rectPantalla entero. Una
                // captura 16:9 sobre un rectPantalla con otra proporción
                // quedaría deformada si se hiciera
                // g.DrawImage(_imagenFondo, rectPantalla) directo.
                var rectImagen = AjustarConservandoProporcion(
                    _imagenFondo.Width, _imagenFondo.Height, rectPantalla);
                g.DrawImage(_imagenFondo, rectImagen);
            }

            using (var penBorde = new Pen(Color.DimGray, 2))
            {
                g.DrawRectangle(penBorde, rectPantalla.X, rectPantalla.Y, rectPantalla.Width, rectPantalla.Height);
            }

            // El marcador activo se dibuja al final para que quede por
            // encima del otro si llegan a superponerse.
            if (_marcadorActivoEsIcono)
            {
                DibujarMarcador(g, _xTexto, _yTexto, ColorTexto, activo: false);
                DibujarMarcador(g, _xIcono, _yIcono, ColorIcono, activo: true);
            }
            else
            {
                DibujarMarcador(g, _xIcono, _yIcono, ColorIcono, activo: false);
                DibujarMarcador(g, _xTexto, _yTexto, ColorTexto, activo: true);
            }
        }

        // Calcula el rectángulo más grande que entra dentro de "contenedor"
        // sin deformar la imagen (mismo criterio que CSS "object-fit: contain"),
        // centrado. El sobrante de "contenedor" que no cubre la imagen queda
        // con el fondo oscuro ya pintado antes (efecto letterbox/pillarbox).
        private static RectangleF AjustarConservandoProporcion(int anchoImagen, int altoImagen, RectangleF contenedor)
        {
            if (anchoImagen <= 0 || altoImagen <= 0)
            {
                return contenedor;
            }

            float escala = Math.Min(
                contenedor.Width / anchoImagen,
                contenedor.Height / altoImagen
            );

            float anchoFinal = anchoImagen * escala;
            float altoFinal = altoImagen * escala;

            float x = contenedor.X + (contenedor.Width - anchoFinal) / 2f;
            float y = contenedor.Y + (contenedor.Height - altoFinal) / 2f;

            return new RectangleF(x, y, anchoFinal, altoFinal);
        }

        private void DibujarMarcador(Graphics g, int x, int y, Color color, bool activo)
        {
            var puntoPanel = ConvertirVirtualAPanel(x, y);
            int radio = activo ? RadioMarcador : RadioMarcador - 3;

            using (var brushMarcador = new SolidBrush(color))
            using (var penMarcador = new Pen(activo ? Color.White : Color.Gray, activo ? 2 : 1))
            {
                var rectMarcador = new RectangleF(
                    puntoPanel.X - radio,
                    puntoPanel.Y - radio,
                    radio * 2,
                    radio * 2
                );
                g.FillEllipse(brushMarcador, rectMarcador);
                g.DrawEllipse(penMarcador, rectMarcador);
            }
        }

        // ============================
        // ARRASTRE DEL MARCADOR ACTIVO
        // ============================
        private void PanelLienzo_MouseDown(object sender, MouseEventArgs e)
        {
            _arrastrando = true;
            _panelLienzo.Capture = true; // permite seguir recibiendo MouseMove aunque el cursor salga del panel
            ActualizarDesdeMouse(e.X, e.Y);
        }

        private void PanelLienzo_MouseMove(object sender, MouseEventArgs e)
        {
            if (_arrastrando)
            {
                ActualizarDesdeMouse(e.X, e.Y);
            }
        }

        private void PanelLienzo_MouseUp(object sender, MouseEventArgs e)
        {
            _arrastrando = false;
            _panelLienzo.Capture = false;
        }

        private void ActualizarDesdeMouse(int panelX, int panelY)
        {
            ConvertirPanelAVirtual(panelX, panelY, out int x, out int y);
            EstablecerPosicionActiva(x, y);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // HudPositionEditorForm
            // 
            this.ClientSize = new System.Drawing.Size(282, 253);
            this.Name = "HudPositionEditorForm";
            this.Load += new System.EventHandler(this.HudPositionEditorForm_Load);
            this.ResumeLayout(false);

        }

        private void HudPositionEditorForm_Load(object sender, EventArgs e)
        {

        }
    }
}