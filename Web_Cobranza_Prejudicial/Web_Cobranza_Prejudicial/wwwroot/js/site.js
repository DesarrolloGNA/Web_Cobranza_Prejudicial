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

async function Funcion_CargarDeuda() {

    var button = document.getElementById("Buscar_Deuda");

    showLoading(button)

    await delay(500);


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
            document.getElementById('_PartialViewBanner').innerHTML = "";
          

            Dibujar_Datatable_Gestiones();

        })
        .catch(error => {
            console.error(error);
        });

    hideLoading(button);

}

function Funcion_CargarInformacion(IDDEUDA) {


    document.getElementById("ID_DEUDA").value = IDDEUDA;


    const params = new URLSearchParams({ ID_DEUDA: IDDEUDA });

    Promise.all([
        cargarPartial(`/Cobranza/_Informacion?${params}`, '_PartialViewInformacion'),
        cargarPartial(`/Cobranza/_Botonera?${params}`, '_PartialViewBotonera'),
        cargarPartial(`/Cobranza/_Gestiones?${params}`, '_PartialViewGestiones'),
        cargarPartial(`/Cobranza/_Banner?${params}`, '_PartialViewBanner')

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
                document.getElementById("BTN_GRABAR_GESTION_PRE").style.visibility = "hidden";
                


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


    document.getElementById("BTN_LLAMAR_DISCADOR").disabled = true;

    var DISCADOR = document.getElementById("DISCADOR");
    DISCADOR.value = 1;


    var ID_DEUDA = document.getElementById("ID_DEUDA").value;
    var BLOQUEO_LEY = document.getElementById("BLOQUEO_LEY").value;

    var SELECT_TELEFONO_DISCADOR = document.getElementById("ID_TELEFONO_DISCADOR");

    var ID_TELEFONO_DISCADOR = SELECT_TELEFONO_DISCADOR.value;
    var TELEFONO_DISCADO = SELECT_TELEFONO_DISCADOR.options[SELECT_TELEFONO_DISCADOR.selectedIndex].text;

    var ID_CARRIER = document.getElementById("ID_CARRIER").value;


    //VISTA DEL FORMULARIO
    var Get = ('/Cobranza/_RegistrarGestion?ID_DEUDA=' + ID_DEUDA + '&DISCADOR=1&BLOQUEO_LEY=' + BLOQUEO_LEY);
    fetch(Get)
        .then(response => response.text())
        .then(data => {
            document.getElementById('_PartialViewRegistrarGestion').innerHTML = data;
            document.getElementById("ID_LOG_DISCADOR").value = "0";
            Dibujar_Select2("#ID_RESPUESTA_LUGAR");



            const telefonoGestion = document.getElementById('ID_TELEFONO');

            if (!telefonoGestion) return;

            telefonoGestion.options.length = 0;

            const option = new Option(TELEFONO_DISCADO, ID_TELEFONO_DISCADOR, true, true);
            telefonoGestion.add(option);

     
            telefonoGestion.style.pointerEvents = "none";


        });



    //EFECTUO LA LLAMADA

    var url = `/api/call/call?NUMERO_TELEFONO=${TELEFONO_DISCADO}&CARRIER=${ID_CARRIER}&ID_DEUDA=${ID_DEUDA}`;


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


            if (data.success) {
                document.getElementById("ID_LOG_DISCADOR").value = data.logId;

            } else {
                document.getElementById("ID_LOG_DISCADOR").value = "0";
                console.error("Error backend:", data.message);
            }
        })
        .catch(error => {
            console.error("Error en fetch:", error);
        });




    SELECT_TELEFONO_DISCADOR.style.pointerEvents = "none";



}

function Funcion_CargarRegistrarGestion() {
    var ID_DEUDA = document.getElementById("ID_DEUDA").value;
    var BLOQUEO_LEY = document.getElementById("BLOQUEO_LEY").value;

    var DISCADOR = document.getElementById("DISCADOR");
    DISCADOR.value = 0;


    var Get = ('/Cobranza/_RegistrarGestion?ID_DEUDA=' + ID_DEUDA + '&BLOQUEO_LEY=' + BLOQUEO_LEY);
    fetch(Get)
        .then(response => response.text())
        .then(data => {
            document.getElementById('_PartialViewRegistrarGestion').innerHTML = data;
            document.getElementById('_PartialViewDiscador').innerHTML = null;
            Dibujar_Select2("#ID_RESPUESTA_LUGAR");

        });
}

let loadingModal;

document.addEventListener("DOMContentLoaded", function () {
    loadingModal = new bootstrap.Modal(document.getElementById('loadingModal'));
});

function showLoading(button) {
    if (button) button.disabled = true;
    loadingModal.show();
}

function hideLoading(button) {
    if (button) button.disabled = false;
    loadingModal.hide();
}

function delay(ms) {
    return new Promise(resolve => setTimeout(resolve, ms));
}

function Funcion_CargarRegistrarTelefono() {




    var Get = ('/Cobranza/_RegistrarTelefono');
    fetch(Get)
        .then(response => response.text())
        .then(data => {
            document.getElementById('_PartialViewRegistrarTelefono').innerHTML = data;
            document.getElementById("ID_DEUDA_TELEFONO").value = document.getElementById("ID_DEUDA").value;
        });
}

function Funcion_CargarRegistrarEmail() {



    
    var Get = ('/Cobranza/_RegistrarEmail');
    fetch(Get)
        .then(response => response.text())
        .then(data => {
            document.getElementById('_PartialViewRegistrarEmail').innerHTML = data;
            document.getElementById("ID_DEUDA_EMAIL").value = document.getElementById("ID_DEUDA").value;
        });
}

function Grabar_Telefono_Pre() {

    var button = document.getElementById("BTN_GRABAR_TELEFONO_PRE");


    var Post = ('/Cobranza/Create_Telefono_Prejudicial');
    var DatosFormulario = new FormData(document.getElementById("GRABARTELEFONOPRE"));
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
                
                document.getElementById("TelefonoPrejudicialAlerta").style.visibility = "visible";
                document.getElementById("TelefonoPrejudicialAlerta").className = "alert alert-success alert-dismissible fade show";
                document.getElementById("TelefonoPrejudicialAlertaCabecera").innerText = "Registro Correcto! ";
                document.getElementById("TelefonoPrejudicialAlertaMensaje").innerText = dataObj.mensaje;
                document.getElementById("BTN_GRABAR_GESTION_J").style.visibility = "hidden";



            } else {
                //muestro la alerta
                document.getElementById("TelefonoPrejudicialAlerta").style.visibility = "visible";
                document.getElementById("TelefonoPrejudicialAlerta").className = "alert alert-danger alert-dismissible fade show";
                document.getElementById("TelefonoPrejudicialAlertaCabecera").innerText = "Error! ";
                document.getElementById("TelefonoPrejudicialAlertaMensaje").innerText = dataObj.mensaje;


            }



        })
        .catch(err => console.error(err));


    return false;
}

function Grabar_Email_Pre() {

    var button = document.getElementById("BTN_GRABAR_EMAIL_PRE");


    var Post = ('/Cobranza/Create_Email_Prejudicial');
    var DatosFormulario = new FormData(document.getElementById("GRABAREMAILPRE"));
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

                document.getElementById("EmailPrejudicialAlerta").style.visibility = "visible";
                document.getElementById("EmailPrejudicialAlerta").className = "alert alert-success alert-dismissible fade show";
                document.getElementById("EmailPrejudicialAlertaCabecera").innerText = "Registro Correcto! ";
                document.getElementById("EmailPrejudicialAlertaMensaje").innerText = dataObj.mensaje;
                document.getElementById("BTN_GRABAR_GESTION_J").style.visibility = "hidden";



            } else {
                //muestro la alerta
                document.getElementById("EmailPrejudicialAlerta").style.visibility = "visible";
                document.getElementById("EmailPrejudicialAlerta").className = "alert alert-danger alert-dismissible fade show";
                document.getElementById("EmailPrejudicialAlertaCabecera").innerText = "Error! ";
                document.getElementById("EmailPrejudicialAlertaMensaje").innerText = dataObj.mensaje;


            }



        })
        .catch(err => console.error(err));


    return false;
}

function Funcion_Mostrar_Telefonos() {

    var DISCADOR = document.getElementById("DISCADOR").value;


    if (DISCADOR == 0) {

        var Telefonos = document.getElementById('ID_TELEFONO');

        while (Telefonos.firstChild) {
            Telefonos.removeChild(Telefonos.firstChild);
        }
        var optionDefault = document.createElement("option");
        optionDefault.value = "";
        optionDefault.text = "Seleccione Contacto";
        Telefonos.appendChild(optionDefault);

        var ID_RESPUESTA_EXCUSA = document.getElementById("ID_RESPUESTA_EXCUSA").value;
        var ID_DEUDA = document.getElementById("ID_DEUDA").value;



        var Get = ('/Cobranza/OBTENER_TELEFONOS_X_ID_RESPUESTA_EXCUSA?ID_DEUDA=' + ID_DEUDA +'&ID_RESPUESTA_EXCUSA=' + ID_RESPUESTA_EXCUSA);
        fetch(Get)
            .then(response => response.text())
            .then(data => {
                var dataObj = JSON.parse(data);
  
                dataObj.forEach(function (option) {
                    var optionElement = document.createElement("option");
                    optionElement.value = option.iD_TELEFONO;
                    optionElement.text = option.telefono;

                    if (option.iD_TELEFONO == 1) {
                        optionElement.selected = true;
                        telefonoCero = true;
                    } else {
                        optionElement.selected = false;
                        telefonoCero = false;

                    }

                    Telefonos.appendChild(optionElement);


                });


                Telefonos.disabled = telefonoCero;


                Dibujar_Select2("#ID_TELEFONO");
            });

    }


}




function Filtrar_Gestiones() {

    const filtro = document.getElementById("FILTRAR_GESTIONES").value;
    const filas = document.querySelectorAll("#Lista_Gestiones tbody tr");

    filas.forEach(fila => {

        const tipo = fila.dataset.gestion;

        fila.style.display = (filtro === "0" || tipo === filtro) ? "" : "none";

    });
}




function CalcularMontoDeuda() {

    let total = 0;

    const checks = document.querySelectorAll("#Lista_Deudas_Ges tbody input[type='checkbox']");

    checks.forEach(chk => {
        if (chk.checked) {
            total += Number(chk.value) || 0;
        }
    });

    document.getElementById("MONTO_TOTAL_DEUDA").value =
        "$" + total.toLocaleString("es-CL");
}


function Seleccionar_Todo(chkHeader) {

    const estado = chkHeader.checked;

    const checks = document.querySelectorAll("#Lista_Deudas_Ges tbody input[type='checkbox']");

    checks.forEach(chk => {
        chk.checked = estado;
    });

    CalcularMontoDeuda();
}


//document.addEventListener("DOMContentLoaded", () => {
//    ActualizarColorEstado(
//        document.getElementById("ID_ESTADO_RESPONSABLE").value
//    );
//});

async function CambiarEstado() {

    const select = document.getElementById("ID_ESTADO_RESPONSABLE");
    const estado = select.value;

    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    if (estado === "7") {

        const form = document.getElementById("Form_estado");
        form.action = "/Home/CerrarSession";
        form.submit();
        return;
    }

    const formData = new FormData();
    formData.append("IdEstado", estado);
    formData.append("__RequestVerificationToken", token);

    await fetch("/Home/CambiarEstado", {
        method: "POST",
        body: formData
    });

    ActualizarColorEstado(estado);
}








function ActualizarColorEstado(estado) {

    const select = document.getElementById("ID_ESTADO_RESPONSABLE");

    select.classList.remove(
        "border-success", "text-success",
        "border-warning", "text-warning",
        "border-danger", "text-danger",
        "border-secondary", "text-secondary"
    );

    switch (estado) {

        case "1":
            select.classList.add("border-success", "text-success");
            break;

        case "2":
        case "3":
        case "4":
            select.classList.add("border-warning", "text-warning");
            break;

        case "5":
            select.classList.add("border-danger", "text-danger");
            break;

        case "6":
            select.classList.add("border-secondary", "text-secondary");
            break;
    }
}



function Seleccionar_Todo_Pagos(chkHeader) {

    const estado = chkHeader.checked;

    const checks = document.querySelectorAll("#Lista_Pagos_Ges tbody input[type='checkbox']");

    checks.forEach(chk => {
        chk.checked = estado;
    });

    CalcularMontoPago();
}



function CalcularMontoPago() {

    let total = 0;

    const checks = document.querySelectorAll("#Lista_Pagos_Ges tbody input[type='checkbox']");

    checks.forEach(chk => {
        if (chk.checked) {
            total += Number(chk.value) || 0;
        }
    });

    document.getElementById("MONTO_TOTAL_PAGO").value =
        "$" + total.toLocaleString("es-CL");
}
