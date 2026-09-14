using System.Collections.Generic;

namespace PHDModManager
{
    internal enum Idioma
    {
        Espanol,
        Ingles
    }

    // Sistema de textos por diccionario en código (NO usa .resx / ApplyResources)
    // a propósito, para evitar el riesgo de corrupción del diseñador que ya
    // se dio antes al renombrar controles. El idioma se elige a mano con
    // BtnIdioma y se persiste en un archivo de config aparte (ver
    // CargarIdiomaGuardado / GuardarIdioma en MainForm.cs).
    internal static class Textos
    {
        public static Idioma IdiomaActual = Idioma.Espanol;

        private static readonly Dictionary<string, string> Es = new Dictionary<string, string>
        {
            // Categorías del acordeón
            ["CatArmas"] = "Skins de Armas",
            ["CatPerks"] = "Skins de Perks",
            ["CatGuantes"] = "Guantes",
            ["CatScripts"] = "Scripts",
            ["CatHud"] = "HUD del Jugador",

            // Barra de herramientas
            ["BtnRecargar"] = "Recargar",
            ["BtnCambiarCarpeta"] = "Cambiar carpeta",
            ["BtnAgregarMod"] = "Añadir skin / script",
            ["BtnIniciarPlutonium"] = "Iniciar Plutonium",
            ["MsgPlutoniumNoEncontrado"] = "No se encontró Plutonium en la ubicación estándar.",
            ["TituloPlutoniumNoEncontrado"] = "Plutonium no encontrado",
            ["MsgErrorIniciarPlutonium"] = "No se pudo iniciar Plutonium.\n\n{0}",
            ["TituloErrorIniciarPlutonium"] = "Error al iniciar Plutonium",

            // Placeholders de búsqueda
            ["PlaceholderBuscarArma"] = "Buscar arma...",
            ["PlaceholderBuscarPerk"] = "Buscar perk...",
            ["PlaceholderBuscarGuante"] = "Buscar guante...",
            ["PlaceholderBuscarScript"] = "Buscar script...",
            ["PlaceholderBuscarHud"] = "Buscar...",

            // Resolución de carpeta de Plutonium
            ["MsgCarpetaGuardadaNoExiste"] = "La carpeta de Plutonium guardada anteriormente ya no existe (¿se movió o reinstalaste el juego?). Vamos a intentar encontrarla de nuevo.",
            ["TituloCarpetaGuardadaNoExiste"] = "Carpeta guardada no encontrada",
            ["MsgCarpetaNoEncontrada"] = "No se encontró la carpeta de Plutonium en la ubicación estándar.\n¿Querés seleccionarla manualmente?",
            ["TituloCarpetaNoEncontrada"] = "Carpeta no encontrada",
            ["MsgInstalarPlutoniumPrimero"] = "Instalá Plutonium primero, o volvé a abrir la app y seleccioná la carpeta.",
            ["DescripcionCarpetaT6"] = "Seleccioná la carpeta 't6' dentro de Plutonium\\storage (contiene 'images' y 'scripts')",
            ["MsgNoSePuedeContinuarSinCarpeta"] = "No se puede continuar sin la carpeta de Plutonium.",

            // Añadir mod
            ["TituloSeleccionarArchivos"] = "Seleccioná el/los archivo(s) de skin (.iwi) o script (.gsc)",
            ["FiltroSkinsScripts"] = "Skins y scripts",
            ["FiltroSkins"] = "Skins",
            ["FiltroScripts"] = "Scripts",
            ["FiltroTodos"] = "Todos los archivos",
            ["ExtensionNoReconocida"] = "{0} (extensión no reconocida, solo .iwi o .gsc)",
            ["MsgAgregadosConErrores"] = "Se agregaron {0} archivo(s).\n\nNo se pudieron agregar:\n{1}",
            ["TituloAlgunosNoAgregados"] = "Algunos archivos no se pudieron agregar",
            ["MsgAgregadosOk"] = "Se agregaron {0} archivo(s) correctamente.",
            ["TituloListo"] = "Listo",

            // Errores al mover/eliminar archivos
            ["MsgErrorMoverMod"] = "No se pudo mover el archivo del mod. Cerrá el juego si está abierto e intentá de nuevo.\n\n{0}",
            ["TituloErrorAplicarMod"] = "Error al aplicar el mod",
            ["MsgErrorMoverScript"] = "No se pudo mover el archivo del script. Cerrá el juego si está abierto e intentá de nuevo.\n\n{0}",
            ["TituloErrorAplicarScript"] = "Error al aplicar el script",
            ["MsgErrorEliminar"] = "No se pudo eliminar el archivo. Cerrá el juego si está abierto e intentá de nuevo.\n\n{0}",
            ["TituloErrorEliminar"] = "Error al eliminar",

            // Confirmación de borrado
            ["MsgConfirmarEliminar"] = "¿Seguro que querés eliminar \"{0}\" definitivamente?\n\nEsto borra el/los archivo(s) del disco y no se puede deshacer.",
            ["TituloConfirmarEliminar"] = "Confirmar eliminación",

            // HUD Player empaquetado (instalar / restaurar original)
            ["BtnInstalarHud"] = "Instalar",
            ["BtnReinstalarHud"] = "Restaurar HUD original empaquetado",
            ["MsgConfirmarSobrescribirHud"] = "Ya tenés tu propia versión de HudPlayerCustomizable.gsc (por ejemplo, con la posición del HUD ya ajustada con el editor).\n\n¿Seguro que querés sobrescribirla con la versión original empaquetada con PHDModManager? Vas a perder los ajustes que le hayas hecho.",
            ["TituloConfirmarSobrescribirHud"] = "Sobrescribir HUD personalizado",
            ["MsgHudReinstalado"] = "Se restauró la versión original de HudPlayerCustomizable.gsc.",
            ["MsgErrorInstalarHud"] = "No se pudo instalar HudPlayerCustomizable.gsc. Cerrá el juego si está abierto e intentá de nuevo.\n\n{0}",
            ["TituloErrorInstalarHud"] = "Error al instalar",

            // Editor de posición del HUD
            ["BtnEditarPosicionHud"] = "Editar posición del HUD",
            ["TituloEditorHud"] = "Editor de posición del HUD",
            ["LblAvisoPosicionAproximada"] = "Posición APROXIMADA. El valor final hay que probarlo y ajustarlo dentro del juego.",
            ["EtiquetaMarcadorIcono"] = "Ícono",
            ["EtiquetaMarcadorTexto"] = "Nombre en pantalla",
            ["LblTextoNombre"] = "Texto que se muestra en el juego:",
            ["LblResolucionJuego"] = "Resolución del juego (ajusta hasta dónde llega el borde derecho real):",
            ["BtnEsquinaArribaIzquierda"] = "Arriba-Izquierda",
            ["BtnEsquinaArribaDerecha"] = "Arriba-Derecha",
            ["BtnEsquinaAbajoIzquierda"] = "Abajo-Izquierda",
            ["BtnEsquinaAbajoDerecha"] = "Abajo-Derecha",
            ["BtnRestablecerOriginal"] = "Restablecer al original",
            ["BtnGuardarPosicion"] = "Guardar posición",
            ["BtnCancelar"] = "Cancelar",
            ["LblPosicionActual"] = "{0} — x = {1}, y = {2}",
            ["MsgArchivoHudNoEncontrado"] = "No se encontró el archivo HudPlayerCustomizable.gsc (ni en scripts activos ni en el backup).",
            ["TituloArchivoHudNoEncontrado"] = "Archivo no encontrado",
            ["MsgPosicionGuardada"] = "Posición guardada.\n\nProbala en el juego para el ajuste final.",
            ["TituloPosicionGuardada"] = "Posición guardada",
            ["MsgErrorGuardarPosicion"] = "No se pudo guardar la posición. Cerrá el juego si está abierto e intentá de nuevo.\n\n{0}",
            ["TituloErrorGuardarPosicion"] = "Error al guardar",
            ["MsgAdvertenciaCantidadReemplazos"] = "Se esperaba encontrar 4 líneas de \"x\" y 4 de \"y\" del ícono (una por cada personaje), pero se encontraron {0} de \"x\" y {1} de \"y\". Revisá el archivo, puede que alguna función tenga un formato distinto.",
            ["TituloAdvertenciaCantidadReemplazos"] = "Cantidad de reemplazos inesperada (ícono)",
            ["MsgAdvertenciaReemplazoTexto"] = "Se esperaba encontrar 1 línea de posición y 1 de texto para el nombre en pantalla, pero se encontraron {0} y {1} respectivamente. Revisá el archivo.",
            ["TituloAdvertenciaReemplazoTexto"] = "Cantidad de reemplazos inesperada (nombre)",
            ["MsgTextoConComillas"] = "El texto no puede contener comillas (\") porque rompería el script. Sacá las comillas e intentá de nuevo.",
            ["TituloTextoInvalido"] = "Texto inválido",

            // Cambiar ícono del HUD
            ["BtnCambiarIconoHud"] = "Cambiar ícono del HUD",
            ["TituloSeleccionarIconoHud"] = "Seleccioná el ícono (.iwi) para el HUD",
            ["FiltroIconoHud"] = "Ícono del HUD",
            ["MsgIconoHudCambiado"] = "Ícono del HUD actualizado.\n\nEste cambio no se puede deshacer desde la app: no hay botón de \"restablecer\". Si más adelante querés volver al ícono anterior, vas a necesitar tener guardada una copia de tu archivo .iwi por tu cuenta.",
            ["MsgErrorCambiarIconoHud"] = "No se pudo cambiar el ícono del HUD. Cerrá el juego si está abierto e intentá de nuevo.\n\n{0}",
            ["TituloErrorCambiarIconoHud"] = "Error al cambiar el ícono",
        };

        private static readonly Dictionary<string, string> En = new Dictionary<string, string>
        {
            ["BtnInstalarHud"] = "Install",
            ["BtnReinstalarHud"] = "Restore packaged original HUD",
            ["MsgConfirmarSobrescribirHud"] = "You already have your own version of HudPlayerCustomizable.gsc (for example, with the HUD position already adjusted with the editor).\n\nAre you sure you want to overwrite it with the original version packaged with PHDModManager? You'll lose any changes you made to it.",
            ["TituloConfirmarSobrescribirHud"] = "Overwrite custom HUD",
            ["MsgHudReinstalado"] = "The original version of HudPlayerCustomizable.gsc was restored.",
            ["MsgErrorInstalarHud"] = "Couldn't install HudPlayerCustomizable.gsc. Close the game if it's open and try again.\n\n{0}",
            ["TituloErrorInstalarHud"] = "Error installing",

            ["CatArmas"] = "Weapon Skins",
            ["CatPerks"] = "Perk Skins",
            ["CatGuantes"] = "Gloves",
            ["CatScripts"] = "Scripts",
            ["CatHud"] = "HUD Player",

            ["BtnRecargar"] = "Reload",
            ["BtnCambiarCarpeta"] = "Change folder",
            ["BtnAgregarMod"] = "Add skin / script",
            ["BtnIniciarPlutonium"] = "Launch Plutonium",
            ["MsgPlutoniumNoEncontrado"] = "Plutonium wasn't found in the standard location.",
            ["TituloPlutoniumNoEncontrado"] = "Plutonium not found",
            ["MsgErrorIniciarPlutonium"] = "Couldn't launch Plutonium.\n\n{0}",
            ["TituloErrorIniciarPlutonium"] = "Error launching Plutonium",

            ["PlaceholderBuscarArma"] = "Search weapon...",
            ["PlaceholderBuscarPerk"] = "Search perk...",
            ["PlaceholderBuscarGuante"] = "Search glove...",
            ["PlaceholderBuscarScript"] = "Search script...",
            ["PlaceholderBuscarHud"] = "Search...",

            ["MsgCarpetaGuardadaNoExiste"] = "The previously saved Plutonium folder no longer exists (did you move it or reinstall the game?). We'll try to find it again.",
            ["TituloCarpetaGuardadaNoExiste"] = "Saved folder not found",
            ["MsgCarpetaNoEncontrada"] = "The Plutonium folder wasn't found in the standard location.\nDo you want to select it manually?",
            ["TituloCarpetaNoEncontrada"] = "Folder not found",
            ["MsgInstalarPlutoniumPrimero"] = "Install Plutonium first, or reopen the app and select the folder.",
            ["DescripcionCarpetaT6"] = "Select the 't6' folder inside Plutonium\\storage (it contains 'images' and 'scripts')",
            ["MsgNoSePuedeContinuarSinCarpeta"] = "Can't continue without the Plutonium folder.",

            ["TituloSeleccionarArchivos"] = "Select the skin (.iwi) or script (.gsc) file(s)",
            ["FiltroSkinsScripts"] = "Skins and scripts",
            ["FiltroSkins"] = "Skins",
            ["FiltroScripts"] = "Scripts",
            ["FiltroTodos"] = "All files",
            ["ExtensionNoReconocida"] = "{0} (unrecognized extension, only .iwi or .gsc)",
            ["MsgAgregadosConErrores"] = "{0} file(s) were added.\n\nCouldn't add:\n{1}",
            ["TituloAlgunosNoAgregados"] = "Some files couldn't be added",
            ["MsgAgregadosOk"] = "{0} file(s) added successfully.",
            ["TituloListo"] = "Done",

            ["MsgErrorMoverMod"] = "Couldn't move the mod file. Close the game if it's open and try again.\n\n{0}",
            ["TituloErrorAplicarMod"] = "Error applying mod",
            ["MsgErrorMoverScript"] = "Couldn't move the script file. Close the game if it's open and try again.\n\n{0}",
            ["TituloErrorAplicarScript"] = "Error applying script",
            ["MsgErrorEliminar"] = "Couldn't delete the file. Close the game if it's open and try again.\n\n{0}",
            ["TituloErrorEliminar"] = "Error deleting",

            ["MsgConfirmarEliminar"] = "Are you sure you want to permanently delete \"{0}\"?\n\nThis deletes the file(s) from disk and cannot be undone.",
            ["TituloConfirmarEliminar"] = "Confirm deletion",

            // HUD position editor
            ["BtnEditarPosicionHud"] = "Edit HUD position",
            ["TituloEditorHud"] = "HUD Position Editor",
            ["LblAvisoPosicionAproximada"] = "APPROXIMATE position. The final value needs to be tested and fine-tuned in-game.",
            ["EtiquetaMarcadorIcono"] = "Icon",
            ["EtiquetaMarcadorTexto"] = "On-screen name",
            ["LblTextoNombre"] = "Text shown in-game:",
            ["LblResolucionJuego"] = "Game resolution (adjusts how far the real right edge goes):",
            ["BtnEsquinaArribaIzquierda"] = "Top-Left",
            ["BtnEsquinaArribaDerecha"] = "Top-Right",
            ["BtnEsquinaAbajoIzquierda"] = "Bottom-Left",
            ["BtnEsquinaAbajoDerecha"] = "Bottom-Right",
            ["BtnRestablecerOriginal"] = "Reset to original",
            ["BtnGuardarPosicion"] = "Save position",
            ["BtnCancelar"] = "Cancel",
            ["LblPosicionActual"] = "{0} — x = {1}, y = {2}",
            ["MsgArchivoHudNoEncontrado"] = "HudPlayerCustomizable.gsc wasn't found (neither in active scripts nor in the backup).",
            ["TituloArchivoHudNoEncontrado"] = "File not found",
            ["MsgPosicionGuardada"] = "Position saved.\n\nTest it in-game for the final fine-tuning.",
            ["TituloPosicionGuardada"] = "Position saved",
            ["MsgErrorGuardarPosicion"] = "Couldn't save the position. Close the game if it's open and try again.\n\n{0}",
            ["TituloErrorGuardarPosicion"] = "Error saving",
            ["MsgAdvertenciaCantidadReemplazos"] = "Expected to find 4 \"x\" lines and 4 \"y\" lines for the icon (one per character), but found {0} of \"x\" and {1} of \"y\". Check the file, some function might have a different format.",
            ["TituloAdvertenciaCantidadReemplazos"] = "Unexpected replacement count (icon)",
            ["MsgAdvertenciaReemplazoTexto"] = "Expected to find 1 position line and 1 text line for the on-screen name, but found {0} and {1} respectively. Check the file.",
            ["TituloAdvertenciaReemplazoTexto"] = "Unexpected replacement count (name)",
            ["MsgTextoConComillas"] = "The text can't contain quotes (\") because it would break the script. Remove them and try again.",
            ["TituloTextoInvalido"] = "Invalid text",

            // Change HUD icon
            ["BtnCambiarIconoHud"] = "Change HUD icon",
            ["TituloSeleccionarIconoHud"] = "Select the HUD icon (.iwi)",
            ["FiltroIconoHud"] = "HUD icon",
            ["MsgIconoHudCambiado"] = "HUD icon updated.\n\nThis change can't be undone from the app: there's no \"reset\" button. If you want to go back to the previous icon later, you'll need your own saved copy of the .iwi file.",
            ["MsgErrorCambiarIconoHud"] = "Couldn't change the HUD icon. Close the game if it's open and try again.\n\n{0}",
            ["TituloErrorCambiarIconoHud"] = "Error changing icon",
        };

        // Devuelve el texto de la clave en el idioma actual. Si falta la
        // clave en el diccionario, devuelve la clave misma (para notar
        // rápido en pantalla qué falta traducir, en vez de tronar).
        public static string T(string clave)
        {
            var tabla = IdiomaActual == Idioma.Ingles ? En : Es;
            return tabla.TryGetValue(clave, out string valor) ? valor : clave;
        }

        // Igual que T, pero aplica string.Format con los argumentos dados.
        // Útil para mensajes que insertan un nombre de archivo, un conteo, etc.
        public static string F(string clave, params object[] args)
        {
            return string.Format(T(clave), args);
        }
    }
}