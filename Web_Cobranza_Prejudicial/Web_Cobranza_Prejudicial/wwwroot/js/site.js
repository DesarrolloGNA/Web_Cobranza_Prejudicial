function Dibujar_Datatable_Gestiones() {
    $('#Lista_Deuda').DataTable(
        {
            "order": [0, 'desc'],
            language: {
                "decimal": "",
                "emptyTable": "Sin Deuda Registrada",
                "info": "Mostrando _START_ a _END_ de _TOTAL_ Entradas",
                "infoEmpty": "Mostrando 0 to 0 of 0 Entradas",
                "infoFiltered": "(Filtrado de _MAX_ total entradas)",
                "infoPostFix": "",
                "thousands": ",",
                "lengthMenu": "Mostrar _MENU_ Entradas",
                "loadingRecords": "Cargando...",
                "processing": "Procesando...",
                "search": "Buscar:",
                "zeroRecords": "Sin Deuda Registrada",
                "paginate": {
                    "first": "Primero",
                    "last": "Ultimo",
                    "next": "Siguiente",
                    "previous": "Anterior"
                }
            },
            fixedHeader: true,
            responsive: true,
            scrollY: '100vh',
            scrollCollapse: true,
        }
    );
    $.fn.dataTable.ext.errMode = 'none';
}


function Funcion_CargarDeuda() {

    const valorBuscar = document.getElementById("VALOR_BUSCAR").value.trim();
    const tipoBusqueda = document.getElementById("TIPO_BUSQUEDA").textContent.trim();

    if (!valorBuscar || !tipoBusqueda) {
        alert("Debe ingresar un valor y seleccionar un tipo de búsqueda");
        return;
    }

    const params = new URLSearchParams({
        TIPO_BUSQUEDA: tipoBusqueda,
        VALOR_BUSCAR: valorBuscar
    });

    fetch(`/Cobranza/_Deuda?${params.toString()}`)
        .then(response => {
            if (!response.ok) {
                throw new Error("Error al cargar la deuda");
            }
            return response.text();
        })
        .then(html => {
            document.getElementById('_PartialViewDeuda').innerHTML = html;
            document.getElementById('_PartialViewInformacion').innerHTML = "";
            document.getElementById('_PartialViewBotonera').innerHTML = "";
            document.getElementById('_PartialViewGestiones').innerHTML = "";
         
        })
        .catch(error => {
            console.error(error);
        });

    Dibujar_Datatable_Gestiones();
}


function Funcion_CargarInformacion(IDDEUDA) {


    document.getElementById("ID_DEUDA").value = IDDEUDA;


    const params = new URLSearchParams({ ID_DEUDA: IDDEUDA });

    Promise.all([
        cargarPartial(`/Cobranza/_Informacion?${params}`, '_PartialViewInformacion'),
        cargarPartial(`/Cobranza/_Botonera`, '_PartialViewBotonera'),
        cargarPartial(`/Cobranza/_Gestiones?${params}`, '_PartialViewGestiones')
           
    ]);
   

    document.getElementById('_PartialViewDeuda').innerHTML = "";

}


function cargarPartial(url, contenedorId) {
    return fetch(url)
        .then(r => {
            if (!r.ok) throw new Error(`Error al cargar ${url}`);
            return r.text();
        })
        .then(html => {
            document.getElementById(contenedorId).innerHTML = html;
        })
        .catch(err => console.error(err));
}




function Funcion_CargarRegistrarGestion() {
    var ID_DEUDA = document.getElementById("ID_DEUDA").value;
    ////oculto la alerta de mensajes
    //document.getElementById("GestionJudicialAlerta").style.visibility = "hidden";

    var Get = ('/Cobranza/_RegistrarGestion?ID_DEUDA=' + ID_DEUDA);
    fetch(Get)
        .then(response => response.text())
        .then(data => {
            document.getElementById('_PartialViewRegistrarGestion').innerHTML = data;
      /*      Dibujar_Select2("#ID_RESPUESTA_CONTACTO");*/
        });

}



function Cant_Caract_Obs() {
    var max = "1000";
    var cadena = document.getElementById("OBSERVACION").value;
    var longitud = cadena.length;
    if (longitud <= max) {
        document.getElementById("OBS_CANT_CARACTERES").innerText = max - longitud;
        if (longitud > (max / 10) * 9) {
            document.getElementById("OBS_CANT_CARACTERES").style.color = "red"
        } else {
            document.getElementById("OBS_CANT_CARACTERES").style.color = "white"
        }
    } else {
        document.getElementById("OBSERVACION").value = cadena.substr(0, max);
    }
}
