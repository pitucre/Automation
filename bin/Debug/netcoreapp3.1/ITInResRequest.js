Ext.define('YZModules.&&ProcessGroup.Modules.&&ProcessName', {
    ProcessName: "&&PNameCN",
    FormService: "&&FormService",
    extend: 'Ext.panel.Panel',
    requires: [
        'YZSoft.bpm.src.ux.FormManager'
    ],
    dlgCfg: {
        dlgModel: 'Tab', //Tab,Window,Dialog
        width: 700,
        height: 650
    },

    constructor: function (config) {
        var me = this;

        //调试时显示模块的权限
        //alert(Ext.encode(config.perm));
        me.store = Ext.create('Ext.data.JsonStore', {
            remoteSort: true,
            pageSize: YZSoft.EnvSetting.pageSize.defaultSize,
            model: 'Ext.data.Model',
            sorters: {
                property: 'taskid',
                direction: 'ASC'
            },
            proxy: {
                type: 'ajax',
                //url: YZSoft.$url(me, 'S_数据服务.ashx'),Handler
                //url: YZSoft.$url(me, '../StoreDataService/ITInResRequest.ashx'),
                url: YZSoft.$url(me, '../StoreDataService/&&ProcessName.ashx'),
                extraParams: {
                    method: 'Zysq'
                },
                reader: {
                    rootProperty: 'children'
                }
            }
        });

        me.grid = Ext.create('Ext.grid.Panel', {
            store: me.store,
            border: false,
            selModel: { mode: 'MULTI' },
            columns: {
                defaults: {
                },
                items: [

             //&&ItemList



             ]
            },
            bbar: Ext.create('Ext.toolbar.Paging', {
                store: me.store,
                displayInfo: true
            }),
            listeners: {
                scope: me,
                rowdblclick: function (grid, record, tr, rowIndex, e, eOpts) {
                    me.edit(record);
                }
            }
        });

        me.btnNew = Ext.create('Ext.button.Button', {
            iconCls: 'yz-glyph yz-glyph-new',
            text: '申报',
            handler: function () {
                me.addNew();
            }
        });

        me.btnEdit = Ext.create('YZSoft.src.button.Button', {
            iconCls: 'yz-glyph yz-glyph-edit',
            text: '查看',
            sm: me.grid.getSelectionModel(),
            updateStatus: function () {
                this.setDisabled(!YZSoft.UIHelper.IsOptEnable(null, me.grid, '', 1, 1));
            },
            handler: function () {
                var sm = me.grid.getSelectionModel(),
                    recs = sm.getSelection() || [];

                if (recs.length != 1)
                    return;

                me.edit(recs[0]);
            }
        });

        me.btnDelete = Ext.create('YZSoft.src.button.Button', {
            iconCls: 'yz-glyph yz-glyph-delete',
            text: '删除',
            sm: me.grid.getSelectionModel(),
            updateStatus: function () {
                this.setDisabled(!YZSoft.UIHelper.IsOptEnable(null, me.grid, '', 1, -1));
            },
            handler: function () {
                me.deleteSelection();
            }
        });

        me.btnExcelExport = Ext.create('YZSoft.src.button.ExcelExportButton', {
            grid: me.grid,
            templateExcel: YZSoft.$url(me, '设备清单模板.xls'), //导出模板，不设置则按缺省方式导出
            params: {},
            fileName: '设备清单',
            allowExportAll: true, //可选项，缺省使用YZSoft.EnvSetting.Excel.AllowExportAll中的设置，默认值false
            //maxExportPages: 10, //可选项，缺省使用YZSoft.EnvSetting.Excel.MaxExportPages中的设置，默认值100
            listeners: {
                beforeload: function (params) {
                    params.ReportDate = new Date()
                }
            }
        });

        var cfg = {
            title: '设备档案信息',
            layout: 'fit',
            border: false,
            items: [me.grid],
            tbar: [me.btnNew, me.btnEdit, '->'/*, {
                xtype: 'button',
                text: '清除搜索条件',
                handler: function () {
                    var params = me.store.getProxy().getExtraParams();
                    params.searchType = '';
                    me.store.reload({ params: { start: 0 } });
                }
            }*/, ' ', {
                xclass: 'YZSoft.src.form.field.Search',
                store: me.store,
                width: 160
            }]
        };

        Ext.apply(cfg, config);
        me.callParent([cfg]);
    },

    onActivate: function (times) {
        if (times == 0)
            this.store.load({
                loadMask: {
                    msg: RS.$('All_Loading'),
                    delay: true
                }
            });
        else
            this.store.reload({ loadMask: false });
    },

    addNew: function () {
        var me = this;
        //YZSoft.bpm.src.ux.FormManager.openFormApplication('Demo/ProductionDevice/ProductionDevice', '', 'New', Ext.apply({
        YZSoft.bpm.src.ux.FormManager.openPostWindow(this.ProcessName, {
            sender: me,
            title: '客户来访提报',
            listeners: {
                submit: function (action, data) {
                    me.store.reload({
                        loadMask: {
                            msg: '保存已成功',
                            delay: 'x'
                        },
                        callback: function () {
                            var rec = me.store.getById(data.Key);
                            if (rec)
                                me.grid.getSelectionModel().select(rec);
                        }
                    });
                }
            }
        }, this.dlgCfg);
    },

    edit: function (rec) {
        var me = this;
        YZSoft.bpm.src.ux.FormManager.openFormApplication('98制品来客/客户来访提报管理部', rec.data.ID, 'Read', Ext.apply({
            sender: me,
            title: '查看',
            params: { tid: rec.data.ID },
            listeners: {
                submit: function (action, data) {
                    me.store.reload({
                        loadMask: {
                            msg: '保存已成功',
                            delay: 'x'
                        }
                    });
                }
            }
        }, this.dlgCfg));
    },

    //Edit: function (item) {
    //    YZSoft.BPM.FormManager.OpenFormApplication('98制品来客/客户来访提报管理部', item.ID, 'Read', true, {
    //        //        YZSoft.BPM.FormManager.OpenPostWindow('制品公司来客提报_变更', '', {
    //        title: '查看',
    //        param: { tid: item.ID },

    //        listeners: {
    //            scope: this,
    //            close: function (form) {
    //                if (form.dialogResult == 'ok') {
    //                    //this.grid.loadSelectId = form.returnValue.key;
    //                    this.store.reload({ params: { start: this.store.cursor } });
    //                }
    //            }
    //        }
    //    });
    //},

    read: function (rec) {
        YZSoft.bpm.src.ux.FormManager.openFormApplication('Demo/ProductionDevice/ProductionDevice', rec.data.id, 'Read', Ext.apply({
            sender: this,
            title: Ext.String.format('设备属性 - {0}', rec.data.Name)
        }, this.dlgCfg));
    },

    deleteSelection: function () {
        var me = this,
            recs = me.grid.getSelectionModel().getSelection(),
            ids = [];

        if (recs.length == 0)
            return;

        Ext.each(recs, function (rec) {
            ids.push(rec.getId());
        });

        Ext.Msg.show({
            title: '删除确认',
            msg: '您确定要删除选中项吗？',
            buttons: Ext.Msg.OKCANCEL,
            defaultFocus: 'cancel',
            icon: Ext.MessageBox.INFO,

            fn: function (btn, text) {
                if (btn != 'ok')
                    return;

                YZSoft.Ajax.request({
                    url: YZSoft.$url(me, 'Services.ashx'),
                    method: 'POST',
                    params: {
                        method: 'Delete'
                    },
                    jsonData: ids,
                    waitMsg: { msg: '正在删除...', target: me.grid },
                    success: function (action) {
                        me.store.reload({
                            loadMask: {
                                msg: Ext.String.format('{0}个对象已删除！', recs.length),
                                delay: 'x'
                            }
                        });
                    },
                    failure: function (action) {
                        var mbox = Ext.Msg.show({
                            title: '错误提示',
                            msg: YZSoft.HttpUtility.htmlEncode(action.result.errorMessage, true),
                            buttons: Ext.Msg.OK,
                            icon: Ext.MessageBox.WARNING
                        });

                        me.store.reload({ mbox: mbox });
                    }
                });
            }
        });
    }
});
