(function ($) {
    $.jqGrid_Filter_Operands = { "eq": "==", "ne": "!", "lt": "<", "le": "<=", "gt": ">", "ge": ">=", "bw": "^", "bn": "!^", "in": "=", "ni": "!=", "ew": "|", "en": "!@", "cn": "~", "nc": "!~", "nu": "#", "nn": "!#" };

    $('.hasMask').each(function () {
        var maskExpression = $(this).attr("data-input-mask");
        $(this).mask(maskExpression, { placeholder: "#" });
    });

    $.clearFilters = function (e) {

        var grid = $(this);
        var colModel = grid.jqGrid('getGridParam', 'colModel');
        colModel.forEach(function (entry) {
            if (entry.hasOwnProperty('searchoptions') && entry.searchoptions.hasOwnProperty('sopt')) {
                var colSearchElement = $('a.soptclass[colname=' + entry.name + ']');
                colSearchElement.attr('soper', entry.searchoptions.sopt[0]);

                var operator = $.jqGrid_Filter_Operands[entry.searchoptions.sopt[0]];
                colSearchElement.text(operator);
            }
        });

        grid[0].clearToolbar();
    };

    $.warn = function (title, message, callback) {
        var popup = BootstrapDialog.show({
            message: message,
            title: title,
            type: BootstrapDialog.TYPE_WARNING,
            buttons: [
                {
                    label: 'OK',
                    cssClass: 'btn-primary',
                    hotkey: 13, // Enter.
                    action: function (dialog) {
                        dialog.close();

                        if (callback) {
                            callback();
                        }
                    }
                }
            ],
            draggable: true
        });
        return popup;
    };

    $.openDialog = function ($elem, _options) {
        var options = _options || {};
        var draggable = true;
        if (options.draggable != undefined) {
            draggable = options.draggable;
        }
        var window = new BootstrapDialog({
            draggable: draggable,
            onshown: function (dialogRef) {
                bindToDialogForm(dialogRef, $elem, options || {});
                if (options.onShown != undefined) {
                    options.onShown(dialogRef);
                }
            }
        });
        var windowId = ($elem.attr('id') || options.id || 'modal') + '_dlg';
        window.setId(windowId);

        var windowTitle = $elem.attr('data-window-title');
        window.setTitle(windowTitle || options.title || "");

        var href = $elem.attr('href');
        if (options.usePost != undefined && options.usePost) {
            var postData = options.postData || {};
            $.post(href || options.url, postData, function (html) {
                onSuccess(window, $elem, options || {}, html);
                window.open();
            });
        } else {
            $.get(href || options.url, function (html) {
                onSuccess(window, $elem, options || {}, html);
                window.open();
            });
        }
        //
        return false;
    };

    function bindToDialogForm(window, $elem, options) {
        jQuery.validator.unobtrusive.parse(document);
        var context = '#' + window.getId();
        var submitForm = $('form', context);
        if (options.onContentLoad != undefined) {
            options.onContentLoad(submitForm);
        }
        
        submitForm.ajaxForm({
            beforeSerialize: function (form) {
                return options.onBeforeSerialize == undefined || options.onBeforeSerialize(form);
            },
            beforeSubmit: function (data, form) {
                return (options.onBeforeSubmit == undefined || options.onBeforeSubmit(form)) && form.valid();
            },
            success: function (response) {
                onSuccess(window, $elem, options, response);
            }
        });
    }

    function onSuccess(window, $elem, options, html) {
        if (options.onSuccessHandler != undefined && typeof (html) === "object") {
            options.onSuccessHandler(window, $elem, options, html);

            return;
        }

        if (html === '$$CLOSE_WINDOW$$') {
            window.close();

            if (options.onSubmit != undefined) {
                options.onSubmit();
            }

            return;
        }

        window.setMessage($(html));
        bindToDialogForm(window, $elem, options || {});
    }

    // fix for jqGrid. removes 'style' attribute which contains a fixed width.
    $(document).ready(function () {
        // $.resetGridWidth();
        $.forEachGrid($.removeClonedGridButtons);
    });

    $(window).resize(function (e) {
        resizeGrids();
    });

    $.onMenuWidthChange = resizeGrids;

    function resizeGrids() {
        $('.ui-jqgrid').each(function () {
            var parentWidth = $(this).parent().width();
            var grid = $(this).find('.ui-jqgrid-btable');
            return grid.jqGrid().setGridWidth(parentWidth, false);
        });
    }

    $.resetGridWidth = function (context) {
        $('.ui-jqgrid, ' +
            '.ui-jqgrid > .ui-jqgrid-view, ' +
            '.ui-jqgrid > .ui-jqgrid-view > .ui-jqgrid-toppager, ' +
            '.ui-jqgrid > .ui-jqgrid-view > .ui-jqgrid-hdiv, ' +
            '.ui-jqgrid > .ui-jqgrid-view > .ui-jqgrid-hdiv > .ui-jqgrid-hbox > .ui-jqgrid-htable, ' +
            '.ui-jqgrid > .ui-jqgrid-view > .ui-jqgrid-bdiv, ' +
            '.ui-jqgrid > .ui-jqgrid-view > .ui-jqgrid-bdiv > > .ui-jqgrid-btable, ' +
            '.ui-jqgrid > .ui-jqgrid-pager', context || $('body')).each(function () {
                var styleAttr = $(this).attr('style');
                if (styleAttr != undefined) {
                    var styleElements = styleAttr.split(';');
                    var newArr = [];
                    styleElements.forEach(function (item) {
                        if (item != "" && item.indexOf("width") == -1) {
                            newArr.push(item);
                        }
                    });

                    if (newArr.length > 0) {
                        var newStyle = newArr.join(' ');
                        $(this).attr('style', newStyle);
                    } else {
                        $(this).removeAttr('style');
                    }
                }
            });
    };

    $.resetGridWidth2 = function (grid, context) {
        var gbox;
        if (grid.hasClass('ui-jqgrid')) {
            gbox = grid;
        } else {
            gbox = grid.closest(".ui-jqgrid");
        }
        var parentContainer = gbox.parent();

        gbox.find('.ui-jqgrid-btable', context).jqGrid().setGridWidth(parentContainer.width());
    };

    $.columnChooser = function (e) {
        var grid = $(this);
        var a = grid.width();
        grid.jqGrid('columnChooser', {
            done: function (perm) {
                if (perm) {
                    grid.setGridWidth(a);
                }
            }
        });
    };

    $.alert = function (title, message) {
        var popup = BootstrapDialog.show({
            message: message,
            title: title,
            type: BootstrapDialog.TYPE_WARNING,
            buttons: [
                {
                    label: 'OK',
                    cssClass: 'btn-primary',
                    hotkey: 13, // Enter.
                    action: function (dialog) {
                        dialog.close();
                    }
                }
            ],
            draggable: true
        });
        return popup;
    };

    $.confirm = function (title, message, callback) {
        var popup = BootstrapDialog.show({
            message: message,
            title: title,
            type: BootstrapDialog.TYPE_WARNING,
            data: {
                callback: callback
            },
            buttons: [{
                label: 'Nu',
                action: function (dialog) {
                    typeof dialog.getData('callback') === 'function' && dialog.getData('callback')(false);
                    dialog.close();
                }
            }, {
                label: 'Da',
                cssClass: 'btn-primary',
                action: function (dialog) {
                    typeof dialog.getData('callback') === 'function' && dialog.getData('callback')(true);
                    dialog.close();
                }
            }],
            draggable: true
        });
        return popup;
    };
    $.unDeleteEntity = function (url) {
        var grid = $(this);
        var selRowId = grid.jqGrid('getGridParam', 'selrow');
        if (selRowId) {
            var dateDeleted = grid.jqGrid('getCell', selRowId, 'DataDeleted');

            if (dateDeleted) {
                var posting = $.post(url, { id: selRowId });
                posting.success(function (data) {
                    grid.trigger('reloadGrid');
                });
            }
        } else {
            $.alert('Avertisment', 'Vă rugăm să selectați un rând.');
        }
    };

    $.showHistory = function (url) {
        var grid = $(this);
        var selRowId = grid.jqGrid('getGridParam', 'selrow');
        if (selRowId) {
            $.openDialog(grid, {
                url: url + '?id=' + selRowId,
                title: 'Istoric'
            });
        } else {
            $.alert('Avertisment', 'Vă rugăm să selectați un rând.');
        }
    };

    $.exportData = function (url) {
        var grid = $(this);
        var dataSetRecords = grid.getGridParam('records');

        function doExport(exportType) {
            var postData = grid.getGridParam('postData');

            var progressDlg = BootstrapDialog.show({
                title: 'Așteptați',
                message: " <br/> <div id='progressbar' class='progress progress-striped active'><div class='progress-bar progress-bar-info active' style='width: 100%;' aria-valuemax='100' aria-valuemin='0' aria-valuenow='100' role='progressbar'><span>In progres</span></div></div> <br/>",
                closable: false,
            });
            $.ajax({
                type: 'POST',
                url: url + "?exportType=" + exportType,
                data: postData,
                success: function (data) {
                    progressDlg.close();
                    document.location.href = data;
                }
            });
        }

        BootstrapDialog.show({
            title: "Selectați varianta de export",
            message: $("<div class=\"row \">" +
                "<div class=\"col-sm-12\">" +
                "<div class=\"radio\">" +
                "<label>" +
                "<input type=\"radio\" name=\"exportType\" value=\"1\" checked=\"\"> Pagină curentă" +
                "<i class=\"fa fa-circle-o small\"></i>" +
                "</label>" +
                "</div>" +
                "<div class=\"radio\">" +
                "<label><input type=\"radio\" name=\"exportType\" value=\"2\" > Tot tabelul<i class=\"fa fa-circle-o small\"></i></label>" +
                "</div>" +
                "</div>" +
                "</div>"),
            buttons: [
                {
                    label: 'OK',
                    cssClass: 'btn-primary',
                    action: function (dialog) {
                        var exportType = $("input:radio:checked").val();
                        if (exportType == 2) {
                            //max Excel allowed rows
                            if (dataSetRecords > 1048576) {
                                $.alert('Avertisment', 'Numărul de rânduri în tabel întrece numărul maxim de randuri permise în tabelele Excel (maxim 1,048,576 rânduri).<br/>Selectați un set mai mic pentru a fi exportat.');
                                dialog.close();
                                return;
                            }

                            if (dataSetRecords > 5000) {
                                $.confirm('Avertisment', 'Numărul de rânduri a fi exportate este mai mare de 5000.<br/>Exportul ar putea dura ceva timp.<br/>Doriți să continuați?',
                                    function (result) {
                                        if (result) {
                                            doExport(exportType);
                                            return true;
                                        } else {
                                            return false;
                                        }
                                    });
                            } else {
                                doExport(exportType);
                            }
                        } else {
                            doExport(exportType);
                        }

                        dialog.close();
                    }
                }, {
                    label: 'Cancel',
                    action: function (dialog) {
                        dialog.close();
                    }
                }
            ]
        });
    };

    $.gridRowAttributes = function (rowData, currentObj, rowId) {
        if (rowData.DataDeleted != undefined && rowData.DataDeleted != '') {
            return {
                'class': 'deletedRecord',
            };
        }

        return {};
    };

    $('.lang-selector').click(function (e) {
        var data = $(this).attr('hreflang');
        $.cookie('_culture', data);
        location.reload();
    });

    $('input[data-val-date]').each(function () {
        var currentCulture = $.cookie('_culture');
        if (currentCulture === undefined || currentCulture == "ro") {
            $(this).attr('data-val-date', 'Introduceți o dată corectă.');
        }
        else if (currentCulture == "ru") {
            $(this).attr('data-val-date', 'Введите верную дату.');
        }
    });

    $('.form-control').tooltip();

    $('span.fa-calendar').each(function () {
        var parent = $(this).closest('div');
        parent.addClass('has-feedback');
    });

    $(".form-control.dp").datepicker({
        changeMonth: true,
        changeYear: true,
        yearRange: "-70:+0"
    });

    $(function () {
        var dateFormat = 'dd.mm.yy';
        $.validator.addMethod('date', function (value, element) {
            if (this.optional(element)) {
                return true;
            }

            try {
                $.datepicker.parseDate(dateFormat, value);
            } catch (err) {
                return false;
            }
            return true;
        });
        $(".datefield").datepicker({ dateFormat: dateFormat, changeYear: true });
        $(".hasDatepicker").datepicker({ dateFormat: dateFormat, changeYear: true });
    });

    $('.hasMask').each(function () {
        var maskExpression = $(this).attr("data-input-mask");
        $(this).mask(maskExpression, { placeholder: "#" });
    }).on('blur', function (e) {
        var value = $(this).val();
        if (value == "") {
            $(this).valid();
        }
    });

    $.ajaxSetup({
        error: function (xhr) {
            var response = jQuery.parseJSON(xhr.responseText);
            BootstrapDialog.show({
                title: 'Eroare',
                type: BootstrapDialog.TYPE_DANGER,
                message: response.Message,
                buttons: [
                    {
                        label: 'OK',
                        cssClass: 'btn-primary',
                        hotkey: 13, // Enter.
                        action: function (dialog) {
                            dialog.close();
                        }
                    }
                ],
                draggable: true
            });
        }
    });

    $('.select2Control').select2();

    $.fn.modal.Constructor.prototype.enforceFocus = function () { };

    $.clearFilters = function (e) {

        var grid = $(this);
        var colModel = grid.jqGrid('getGridParam', 'colModel');
        colModel.forEach(function (entry) {
            if (entry.hasOwnProperty('searchoptions') && entry.searchoptions.hasOwnProperty('sopt')) {
                var colSearchElement = $('a.soptclass[colname=' + entry.name + ']');
                colSearchElement.attr('soper', entry.searchoptions.sopt[0]);

                var operator = $.jqGrid_Filter_Operands[entry.searchoptions.sopt[0]];
                colSearchElement.text(operator);
            }
        });

        grid[0].clearToolbar();
    };

    $.forEachGrid = function (callback) {
        $('.ui-jqgrid').each(function () {
            if (callback !== undefined && typeof (callback) == "function") {
                var grid = $(this).find('.ui-jqgrid-btable').jqGrid();
                callback(grid);
            }
            ;
        });
    };

    $.removeClonedGridButtons = function (grid) {
        var gridId = grid.attr('id');
        var bottomPagerDiv = $('div#' + gridId + 'Pager')[0];
        $('#del_' + gridId, bottomPagerDiv).remove();
        $('#search_' + gridId, bottomPagerDiv).remove();
        $('#refresh_' + gridId, bottomPagerDiv).remove();
        $('#add_' + gridId, bottomPagerDiv).remove();
        $('#edit_' + gridId, bottomPagerDiv).remove();
        $('#' + gridId + 'Pager_center', bottomPagerDiv).remove();
    };



})(jQuery);

function ShowConfirmDialog(message, confirmCallback, cssClass) {
    BootstrapDialog.yes_no_confirm({
        draggable: true,
        cssClass: (cssClass != null) ? cssClass : 'warning-dialog',
        title: 'Atenție',
        message: message,
        callback: confirmCallback
    });
}

function ShowAlert(message, title, cssClass) {
    BootstrapDialog.show({
        draggable: true,
        cssClass: (typeof cssClass != 'undefined') ? cssClass : 'info-dialog',
        title: (typeof title != 'undefined') ? title : 'Info',
        message: message,
        closable: false,
        buttons: [
            {
                label: 'Închide',
                action: function (dialogItself) {
                    dialogItself.close();
                }
            }
        ]
    });
}

var waitingDialog = waitingDialog || (function ($) {
    'use strict';

    // Creating modal dialog's DOM
    var $dialog = $(
        '<div class="modal fade" data-backdrop="static" data-keyboard="false" tabindex="-1" role="dialog" aria-hidden="true" style="padding-top:15%; overflow-y:visible;">' +
        '<div class="modal-dialog modal-m">' +
        '<div class="modal-content">' +
        '<div class="modal-header"><h3 style="margin:0;"></h3></div>' +
        '<div class="modal-body">' +
        '<div class="progress progress-striped active" style="margin-bottom:0;"><div class="progress-bar" style="width: 100%"></div></div>' +
        '</div>' +
        '</div></div></div>');

    return {
		/**
		 * Opens our dialog
		 * @param message Custom message
		 * @param options Custom options:
		 * 				  options.dialogSize - bootstrap postfix for dialog size, e.g. "sm", "m";
		 * 				  options.progressType - bootstrap postfix for progress bar type, e.g. "success", "warning".
		 */
        show: function (message, options) {
            // Assigning defaults
            if (typeof options === 'undefined') {
                options = {};
            }
            if (typeof message === 'undefined') {
                message = 'Loading';
            }
            var settings = $.extend({
                dialogSize: 'm',
                progressType: '',
                onHide: null // This callback runs after the dialog was hidden
            }, options);

            // Configuring dialog
            $dialog.find('.modal-dialog').attr('class', 'modal-dialog').addClass('modal-' + settings.dialogSize);
            $dialog.find('.progress-bar').attr('class', 'progress-bar');
            if (settings.progressType) {
                $dialog.find('.progress-bar').addClass('progress-bar-' + settings.progressType);
            }
            $dialog.find('h3').text(message);
            // Adding callbacks
            if (typeof settings.onHide === 'function') {
                $dialog.off('hidden.bs.modal').on('hidden.bs.modal', function (e) {
                    settings.onHide.call($dialog);
                });
            }
            // Opening dialog
            $dialog.modal();
        },
		/**
		 * Closes dialog
		 */
        hide: function () {
            $dialog.modal('hide');
        }
    };

})(jQuery);
