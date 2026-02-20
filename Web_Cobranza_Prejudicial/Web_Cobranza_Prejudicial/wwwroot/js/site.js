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
            document.getElementById('_PartialViewDiscador').innerHTML = null;
                  Dibujar_Select2("#ID_RESPUESTA_LUGAR");

        });

}

function Funcion_CargarDiscador() {
    var ID_DEUDA = document.getElementById("ID_DEUDA").value;

    var Get = ('/Cobranza/_Discador?ID_DEUDA=' + ID_DEUDA);
    fetch(Get)
        .then(response => response.text())
        .then(data => {
            document.getElementById('_PartialViewDiscador').innerHTML = data;
            document.getElementById('_PartialViewRegistrarGestion').innerHTML = null;


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



//carga formato Search a los Select de bootstrap 5
function Dibujar_Select2(ID_SELECT) {
    $(ID_SELECT).each(function () {
        $(this).select2({
            theme: "bootstrap-5",
            width: $(this).data('width') ? $(this).data('width') : $(this).hasClass('w-100') ? '100%' : 'style',
            placeholder: $(this).data('placeholder'),
            dropdownParent: $(this).parent(),
        });
    });
}



function Funcion_Cargar_Contacto_x_Lugar() {

    var SelectContacto = document.getElementById('ID_RESPUESTA_CONTACTO');
    while (SelectContacto.firstChild) {
        SelectContacto.removeChild(SelectContacto.firstChild);
    }
    var optionDefault = document.createElement("option");
    optionDefault.value = "";
    optionDefault.text = "Seleccione Contacto";
    SelectContacto.appendChild(optionDefault);

    var ID_RESPUESTA_LUGAR = document.getElementById("ID_RESPUESTA_LUGAR").value;
    var Get = ('/Cobranza/OBTENER_CONTACTO_X_LUGAR?ID_RESPUESTA_LUGAR=' + ID_RESPUESTA_LUGAR);
    fetch(Get)
        .then(response => response.text())
        .then(data => {
            var dataObj = JSON.parse(data);
            dataObj.forEach(function (option) {
                var optionElement = document.createElement("option");
                optionElement.value = option.iD_RESPUESTA_CONTACTO;
                optionElement.text = option.contacto;
                SelectContacto.appendChild(optionElement);
            });

            Dibujar_Select2("#ID_RESPUESTA_CONTACTO");
        });
}




function Funcion_Cargar_Excusa_x_Contacto() {

    var SelectExcusa = document.getElementById('ID_RESPUESTA_EXCUSA');
    while (SelectExcusa.firstChild) {
        SelectExcusa.removeChild(SelectExcusa.firstChild);
    }
    var optionDefault = document.createElement("option");
    optionDefault.value = "";
    optionDefault.text = "Seleccione Contacto";
    SelectExcusa.appendChild(optionDefault);

    var ID_RESPUESTA_CONTACTO = document.getElementById("ID_RESPUESTA_CONTACTO").value;
    var Get = ('/Cobranza/OBTENER_EXCUSA_X_ID_RESPUESTA_CONTACTO?ID_RESPUESTA_CONTACTO=' + ID_RESPUESTA_CONTACTO);
    fetch(Get)
        .then(response => response.text())
        .then(data => {
            var dataObj = JSON.parse(data);

            dataObj.forEach(function (option) {
                var optionElement = document.createElement("option");
                optionElement.value = option.iD_RESPUESTA_EXCUSA;
                optionElement.text = option.respuestA_EXCUSA;
                SelectExcusa.appendChild(optionElement);
            });

            Dibujar_Select2("#ID_RESPUESTA_EXCUSA");
        });
}




function Grabar_Gestion_Pre() {
    var Post = ('/Cobranza/Create_Gestion_Prejudicial');
    var DatosFormulario = new FormData(document.getElementById("GRABARGESTIONPRE"));
    fetch(Post, {
        method: "POST",
        body: DatosFormulario
    })
        .then(res => {
            if (res.status != 200) { throw new Error("Bad Server Response"); }
            return res.text();
        })
        .then(res => {

            //Serializo el Json
            var dataObj = JSON.parse(res);
            //aqui la logica si registra o no
            if (dataObj.returN_VALUE > 0) {
                var ID_DEUDA = document.getElementById("ID_DEUDA").value;
                Funcion_CargarInformacion(ID_DEUDA);

                //muestro la alerta
                document.getElementById("GestionPrejudicialAlerta").style.visibility = "visible";
                document.getElementById("GestionPrejudicialAlerta").className = "alert alert-success alert-dismissible fade show";
                document.getElementById("GestionPrejudicialAlertaCabecera").innerText = "Registro Correcto! ";
                document.getElementById("GestionPrejudicialAlertaMensaje").innerText = dataObj.mensaje;
                document.getElementById("BTN_GRABAR_GESTION_J").style.visibility = "hidden";
                



            } else {
                //muestro la alerta
                document.getElementById("GestionPrejudicialAlerta").style.visibility = "visible";
                document.getElementById("GestionPrejudicialAlerta").className = "alert alert-danger alert-dismissible fade show";
                document.getElementById("GestionPrejudicialAlertaCabecera").innerText = "Error! ";
                document.getElementById("GestionPrejudicialAlertaMensaje").innerText = dataObj.mensaje;
            }
        })
        .catch(err => console.error(err));
    return false;
}



function LLamar() {



    var ID_DEUDA = document.getElementById("ID_DEUDA").value;
    var ID_TELEFONO = document.getElementById("ID_TELEFONO").value;
    var ID_CARRIER = document.getElementById("ID_CARRIER").value;

    //VISTA DEL FORMULARIO
    var Get = ('/Cobranza/_RegistrarGestion?ID_DEUDA=' + ID_DEUDA);
    fetch(Get)
        .then(response => response.text())
        .then(data => {
            document.getElementById('_PartialViewRegistrarGestion').innerHTML = data;
            document.getElementById("ID_LOG_DISCADOR").value = "0";
            Dibujar_Select2("#ID_RESPUESTA_LUGAR");
        });
        
    //EFECTUO LA LLAMADA

    var url = `/api/call/call?NUMERO_TELEFONO=${ID_TELEFONO}&CARRIER=${ID_CARRIER}&ID_DEUDA=${ID_DEUDA}`;


    fetch(url, {
        method: 'POST'
    })
        .then(res => {
            if (!res.ok) {
                throw new Error("Error HTTP " + res.status);
            }
            return res.json();
        })
        .then(data => {
            console.log("Respuesta completa:", data);

            if (data.success) {
                document.getElementById("ID_LOG_DISCADOR").value = data.logId;
                console.log("ID LOG:", data.logId);
                console.log("Respuesta AMI:", data.amiResponse);
            } else {
                document.getElementById("ID_LOG_DISCADOR").value = "0";
                console.error("Error backend:", data.message);
            }
        })
        .catch(error => {
            console.error("Error en fetch:", error);
        });




}




function Funcion_CargarRegistrarGestion() {
    var ID_DEUDA = document.getElementById("ID_DEUDA").value;

    var Get = ('/Cobranza/_RegistrarGestion?ID_DEUDA=' + ID_DEUDA);
    fetch(Get)
        .then(response => response.text())
        .then(data => {
            document.getElementById('_PartialViewRegistrarGestion').innerHTML = data;
            document.getElementById('_PartialViewDiscador').innerHTML = null;
            Dibujar_Select2("#ID_RESPUESTA_LUGAR");

        });

}
