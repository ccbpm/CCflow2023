'use strict';
window.onload = function () {
    window.vm = new Vue({
        el: '#g-app-main',
        data: function () {
            return {
                tabsList: [], // 打开的tab页面
                showPagesAction: false, // 打开tabs操作选项
                selectedTabsIndex: -1, // 当前所选的tab 索引
                sideBarOpen: true,
                inFullScreenMode: false, // 是否处于全屏模式
                showThemePicker: false, // 显示主题选择器
                classicalLayout: true,
                menuTreeData: [], // 目录数据
                subMenuData: [], // 二级目录数据
                selectedTopMenuIndex: 0,
                subMenuTitle: '', // 二级目录标题
                selectedId: '-1', // 当前激活的id
                // 是否开启刷新记忆tab功能
                is_remember: false,
                webUser: new WebUser(),
                isAdmin: false,
                showShortCut: false,
                showShortOA: false,
                showUserActions: false,
                tabDropdownVisible: false,
                top: 0,
                left: 0,
                closeTimeout: null,
                SystemName: "驰骋BPM",
                SystemLogo: "./image/logo.png",
                IsShowFast: true,
                IsShowOA: true,
                IsShowRefresh: true,
                IsShowFullScreen: true,
                IsShowTheme: true,
                IsShowFlexible: true,
                IsShowTwoLevelMenu: false,
                msgListener: null,
                plant:"CCFlow"

            };
        },
        computed: {
            contextMenuStyle: function () {
                return {
                    position: 'fixed',
                    zIndex: 9999,
                    top: (this.top || 0) + 'px',
                    left: (this.left || 0) + 'px',
                    padding: '0 10px',
                    border: '1px solid #eee'
                };
            },
        },
        methods: {
            initIM: function () {
                var webUser = new WebUser();
                var orgNo = webUser.OrgNo;
                if (orgNo == "" || orgNo == null) {
                    orgNo = "ccflow";
                }
                var bgc = JSON.parse(localStorage.getItem("themeColorInfo"));
                umclient.init({
                    No: webUser.No,
                    Name: webUser.Name,
                    Password: '',
                    FG_OrgNo: orgNo,//必填，不能为空
                    FG_OrgName: webUser.OrgName,
                    FG_DeptNo: webUser.FK_Dept,
                    FG_DeptName: webUser.FK_DeptName,
                    isShowVideo: true, // 是否启用音视频通话 
                    headerBgc: bgc.selectedMenu
                }, '#iconIM')
            },
            openTabDropdownMenu: function (e) {
                this.tabDropdownVisible = true;
                this.top = e.pageY;
                this.left = e.pageX;
            },
            selectTopMenu: function (index) {
                if (this.classicalLayout) return;
                this.selectedTopMenuIndex = index;
                this.selectedSubIndex = -1;
                if (this.menuTreeData.length > 0) {
                    this.subMenuData = this.menuTreeData[index];
                    this.subMenuTitle = this.menuTreeData[index].Name;
                    if (this.subMenuTitle.length > 4)
                        $(".line").css("width", (70 - (this.subMenuTitle.length - 4) * 8) + "px");
                    this.bindDropdown(this.subMenuData.type)
                }
                this.sideBarOpen = true
                this.initChildContextMenu()
            },
            fullScreenOpen: function (uri, name) {
                this.changeFullScreenStatus()
                var w = screen.width,
                    h = screen.height
                layer.open({
                    type: 2,
                    title: name,
                    content: [uri, 'no'],
                    area: [w + 'px', h + 'px'],
                    offset: 'rb',
                    shadeClose: true
                })
            },
            changeFullScreenStatus: function () {
                if (this.inFullScreenMode) {
                    this.exitFullScreen()
                    return
                }
                this.fullScreen()
            },
            // 全屏事件
            fullScreen: function () {
                var element = window.document
                    .documentElement; //若要全屏页面中div，var element= document.getElementById("divID");
                //IE 10及以下ActiveXObject
                if (window.ActiveXObject) {
                    var WsShell = new ActiveXObject('WScript.Shell')
                    WsShell.SendKeys('{F11}');
                }
                //HTML W3C 提议
                else if (element.requestFullScreen) {
                    element.requestFullScreen();
                }
                //IE11
                else if (element.msRequestFullscreen) {
                    element.msRequestFullscreen();
                }
                // Webkit (works in Safari5.1 and Chrome 15)
                else if (element.webkitRequestFullScreen) {
                    element.webkitRequestFullScreen();
                }
                // Firefox (works in nightly)
                else if (element.mozRequestFullScreen) {
                    element.mozRequestFullScreen();
                }
                this.inFullScreenMode = true
            },
            exitFullScreen: function () {
                var element = document
                    .documentElement; //若要全屏页面中div，var element= document.getElementById("divID");
                //IE ActiveXObject
                if (window.ActiveXObject) {
                    var WsShell = new ActiveXObject('WScript.Shell')
                    WsShell.SendKeys('{F11}');
                }
                //HTML5 W3C 提议
                else if (element.requestFullScreen) {
                    document.exitFullscreen();
                }
                //IE 11
                else if (element.msRequestFullscreen) {
                    document.msExitFullscreen();
                }
                // Webkit (works in Safari5.1 and Chrome 15)
                else if (element.webkitRequestFullScreen) {
                    document.webkitCancelFullScreen();
                }
                // Firefox (works in nightly)
                else if (element.mozRequestFullScreen) {
                    document.mozCancelFullScreen();
                }
                this.inFullScreenMode = false
            },
            // 改变侧边栏大小
            resizeSideBar: function () {
                this.sideBarOpen = !this.sideBarOpen
            },
            // 上一个页面
            toLeftPage: function () {
                this.$nextTick(function () {
                    var iframeTabs = this.$refs['iframe-tabs']
                    var offsetWidth = iframeTabs.offsetWidth
                    var scrollWidth = iframeTabs.scrollWidth
                    var offsetLeft = iframeTabs.offsetLeft
                    if (scrollWidth <= offsetWidth) {
                        return
                    }
                    if (offsetLeft < 0) {
                        var leftDistance = offsetLeft + offsetWidth
                        if (leftDistance > 0) {
                            leftDistance = 0
                        }
                        iframeTabs.style.left = leftDistance + 'px'
                    }

                })

            },
            // 下一个页面
            toRightPage: function () {
                this.$nextTick(function () {
                    var iframeTabs = this.$refs['iframe-tabs']
                    var offsetWidth = iframeTabs.offsetWidth
                    var scrollWidth = iframeTabs.scrollWidth
                    var offsetLeft = iframeTabs.offsetLeft
                    if (scrollWidth <= offsetWidth) {
                        return
                    }
                    if (Math.abs(offsetLeft) < scrollWidth - offsetWidth) {
                        iframeTabs.style.left = offsetLeft - offsetWidth + 'px'
                    }
                })
            },
            // 重载当前页面
            reLoadCurrentPage: function () {
                this.$nextTick(function () {
                    if (this.selectedTabsIndex === -1) {
                        this.$refs['iframe-home'].contentWindow.location.reload();
                        return
                    }
                    var src = this.tabsList[this.selectedTabsIndex].src;
                    var oldSrc = this.$refs['iframe-' + this.selectedTabsIndex][0].contentWindow.location.href;
                    if (oldSrc.indexOf(src) != -1)
                        this.$refs['iframe-' + this.selectedTabsIndex][0].contentWindow.location
                            .reload()
                    else
                        this.$refs['iframe-' + this.selectedTabsIndex][0].contentWindow.location.href = src;
                })
            },
            // 关闭当前标签页
            closeCurrentTabs: function (index, isShowTip) {
                isShowTip = isShowTip || false;
                if (index == undefined)
                    index = this.selectedTabsIndex;

                var cur = this.tabsList[index];
                var src = cur.src || "";
                var _this = this;
                if ((src.indexOf('MyFlowGener') != -1 || src.indexOf('MyFlowTree') != -1
                    || src.indexOf('MyDict') != -1 || src.indexOf('MyBill') != -1) && src.indexOf('IsReadonly=1') == -1 && isShowTip) {
                    layer.confirm('关闭当前页面,请检查是否已保存填写的内容', function (closeIndex) {
                        layer.close(closeIndex);
                        _this.tabsList.splice(index, 1)
                        setTimeout(function () {
                            if (_this.tabsList.length > index) {
                                _this.selectedTabsIndex = index
                            } else {
                                _this.selectedTabsIndex = index - 1
                            }
                            if (_this.selectedTabsIndex == -1)
                                _this.selectedId = "";
                            else
                                _this.selectedId = _this.tabsList[_this.selectedTabsIndex].no;
                            var cur = _this.tabsList[_this.selectedTabsIndex];
                            if ((cur != undefined && cur.name == "待办") || _this.selectedTabsIndex == -1)
                                _this.reLoadCurrentPage();
                        }, 100)
                    });
                } else {
                    _this.tabsList.splice(index, 1)
                    setTimeout(function () {
                        if (_this.tabsList.length > index) {
                            _this.selectedTabsIndex = index
                        } else {
                            _this.selectedTabsIndex = index - 1
                        }
                        if (_this.selectedTabsIndex == -1)
                            _this.selectedId = "";
                        else
                            _this.selectedId = _this.tabsList[_this.selectedTabsIndex].no;
                        var cur = _this.tabsList[_this.selectedTabsIndex];
                        if ((cur != undefined && cur.name == "待办") || _this.selectedTabsIndex == -1)
                            _this.reLoadCurrentPage();
                    }, 100)
                }

               
            },
            closeTabByName: function (name) {
                if (name == null || name == undefined || name == "")
                    return;
                if (this.tabsList.length == 0)
                    return;
                //获取当前标签所在的位置
                var index = -1;
                $.each(this.tabsList, function (i, item) {
                    if (item.name == name) {
                        index = i;
                        return false;
                    }
                })
                if (index == -1)
                    return;
                this.tabsList.splice(index, 1)
                var _this = this
                setTimeout(function () {
                    if (_this.tabsList.length > index) {
                        _this.selectedTabsIndex = index
                    } else {
                        _this.selectedTabsIndex = index - 1
                    }
                    if (_this.selectedTabsIndex == -1)
                        _this.selectedId = "";
                    else
                        _this.selectedId = _this.tabsList[_this.selectedTabsIndex].no;
                    var cur = _this.tabsList[_this.selectedTabsIndex];
                    if ((cur != undefined && cur.name == "待办") || _this.selectedTabsIndex == -1)
                        _this.reLoadCurrentPage();
                }, 100)
            },

            // 关闭所有
            closeAllTabs: function () {
                this.tabsList = []
                this.selectedTabsIndex = -1;
                this.selectedId = "";
                this.$nextTick(function () {
                    this.$refs['iframe-tabs'].style.left = 0 + 'px'
                })
            },
            // 关闭其他
            closeOtherTabs: function () {
                if (this.tabsList.length === 0) return
                var tab = this.tabsList[this.selectedTabsIndex]
                this.tabsList = [tab]
                this.selectedTabsIndex = 0
            },
            // 处理tab滚动
            handleTabScroll: function () {
                // 待实现
                // this.$nextTick(function() {
                //     var tabs = this.$refs['iframe-tabs']
                //     var elLeft = tabs.querySelector('.layui-this').offsetLeft
                //     if (elLeft >= 0 && elLeft <= Math.abs(tabs.offsetLeft)) {
                //         return
                //     } else {
                //         tabs.style.left = -(elLeft - elWidth) + 'px'
                //     }
                // })
            },
            openTabByMenu: function (menu, alignRight) {

                //写入日志.
                UserLogInsert("MenuClick", menu.Name + "@" + menu.Icon + "@" + menu.Url);

                this.openTab(menu.Name, menu.Url,menu.No, alignRight);
            },
            openTab: function (name, src, no, alignRight) {
                if ((src.indexOf("https:") != -1 || src.indexOf("http") != -1)
                    && src.indexOf(basePath) == -1) {
                    var bschitchat = window.open(src, name, 'toolbar=no,menubar=no,scrollbars=auto,resizeable=no,location=no,status=no');
                    bschitchat.moveTo(0, 0);
                    bschitchat.resizeTo(screen.availWidth, screen.availHeight);
                    bschitchat.outerWidth = screen.availWidth;
                    bschitchat.outerHeight = screen.availHeight;
                    return;
                }
                // if (src.indexOf("https:") == -1 && src.indexOf("http") == -1) {
                //     src = basePath + src;
                // }
                //如果发起实体类的流程，是通过一个页面中专过去的.
                /*
                 *  /WF/CCBill/Opt/StartFlowByNewEntity.htm
                 *  这里不解析特殊的业务逻辑, 让页面解析。
                 *
                 */
                if (this.tabsList.length >= 30) {
                    layer.alert('最多可以打开30个标签页~');
                    return;
                }
                var obj = {
                    no:no||"",
                    name: name,
                    src: src
                }

                var idx = this.checkExist(obj)
                if (idx > -1) {
                    this.selectedTabsIndex = idx
                    this.reLoadCurrentPage()
                    return
                }

                if (alignRight) {
                    this.tabsList.splice(this.selectedTabsIndex + 1, 0, obj)
                    this.selectedTabsIndex = this.selectedTabsIndex + 1
                } else {
                    this.tabsList.push(obj)
                    this.selectedTabsIndex = this.tabsList.length - 1

                }
                // if (this.tabsList.length > 5)
                //     this.handleTabScroll()
            },
            changeSelectTab: function (index,item) {
                this.selectedTabsIndex = index;
                if (index == -1)
                    this.selectedId == "";
                else
                    this.selectedId = item.no;
                var cur = this.tabsList[this.selectedTabsIndex];
                if ((cur != undefined && cur.name == "待办") || index==-1)
                    this.reLoadCurrentPage();
            },
            checkExist: function (obj) {
                for (var i = 0; i < this.tabsList.length; i++) {
                    var item = this.tabsList[i]
                    if (item.name === obj.name && item.src === obj.src) {
                        return i
                    }
                }
                return -1
            },
            foldMenus: function (menus, c, ev) {
                for (var i = 0; i < menus.length; i++) {
                    var item = menus[i]
                    if (item.No === c.No) {
                        item.IsOpen = !item.IsOpen
                        continue
                    }
                    // item.open = false
                    item.IsOpen = false
                }

            },
            generatePickerBody: function () {
                var tag = "<div style=\"padding-left:10px;padding-right:10px;padding-top:10px\">" +
                    "<form class=\"layui-form layui-form-pane\" action=\"\">" +
                    "   <div class=\"layui-form-item\" pane>" +
                    "   <label class=\"layui-form-label\">\u5206\u680F\u5E03\u5C40</label>" +
                    "<div class=\"layui-input-block\">";
                if (this.IsShowTwoLevelMenu == true || getPortalConfigByKey("IsClassicalLayout", true) == false)
                    tag += "<input type=\"checkbox\" lay-skin=\"switch\" lay-text=\"\u5F00\u542F|\u5173\u95ED\" lay-filter=\"layout\" disabled checked>" + "</div>" + "</div>" + "</form>" + "</div>" + "<hr class=\"layui-border-black\">" + "<div class='theme-picker'>";
                else
                    tag += "<input type=\"checkbox\" lay-skin=\"switch\" lay-text=\"\u5F00\u542F|\u5173\u95ED\" lay-filter=\"layout\" ".concat(this.classicalLayout ? '' : 'checked', ">" + "</div>" + "</div>" + "</form>" + "</div>" + "<hr class=\"layui-border-black\">" + "<div class='theme-picker'>");
                for (var key in themeData) {
                    if (themeData.hasOwnProperty(key)) {
                        var item = themeData[key]
                        tag += "\n                    <div class=\"theme\" style=\"background-color: ".concat(item.logo, "\" onclick=\"chooseTheme('").concat(key, "')\">\n                        ").concat(item.alias, "\n                    </div>\n                    ");
                    }
                }
                tag += '</div>'
                return tag;
            },
            openThemePicker: function () {
                var _this = this
                var height = window.innerHeight * 0.8
                layer.open({
                    type: 1,
                    title: '颜色与布局',
                    content: this.generatePickerBody(),
                    area: ['300px', height + 'px'],
                    offset: 'rb',
                    shadeClose: true
                })
                layui.use('form', function () {
                    var form = layui.form
                    form.render()
                    form.on("switch(layout)", function (e) {
                        _this.classicalLayout = !_this.classicalLayout
                        _this.updateLayout()
                    })
                })

            },
            updateLayout: function () {
                var layout = document.getElementById("layout-data")
                if (!this.classicalLayout) {
                    try {
                        this.classicalLayout = false
                        this.selectTopMenu(0)
                        layout.innerHTML = "\n                        .g-admin-layout .layui-side{                        .g-admin-layout .layui-logo, .layui-side-menu .layui-nav{\n                                                      position: absolute;\n                            \n                            height: 50px;\n                            line-height:50px;\n                            color: #333;\n                            box-shadow: none;\n                        }\n                        .layui-side-menu .layui-side-scroll{\n                            background-color: white;\n                                                   }\n                        .g-admin-pagetabs, .g-admin-layout .layui-body, .g-admin-layout .layui-footer, .g-admin-layout .g-layout-left{\n                            left:285px;\n                        }\n                        .layui-side-menu .layui-nav .layui-nav-item a{\n                            height: 30px;\n                            line-height: 30px;\n                            color:#5f626e;\n                            display: flex;\n                            align-items: center;\n                        }\n                        .layui-side-menu .layui-nav .layui-nav-item .layui-icon{\n                            margin-top: -14px;\n                        }          \n                    ";
                        localStorage.setItem("classicalLayout", "0")

                    } catch (error) {
                        layer.msg('加载失败')
                    }
                } else {
                    this.classicalLayout = true
                    layout.innerHTML = ''
                    localStorage.setItem("classicalLayout", "1")
                }
                var color = localStorage.getItem("themeColor")
                chooseTheme(color);

            },
            refreshMenuTree: function (data) {
                console.log(data)
                this.menuTreeData = new MenuConvertTools(this.webUser, data).convertToTreeData();
                //如果是二级菜单显示去掉系统菜单
                if (this.IsShowTwoLevelMenu == true && this.menuTreeData.length > 0) {
                    var modulesNode = [];
                    this.menuTreeData.forEach(systemItem => {
                        modulesNode = $.merge(modulesNode, systemItem.children);

                    });

                    this.menuTreeData = modulesNode;
                }
                layer.close(loading);
                // var color = localStorage.getItem("themeColor")
                // chooseTheme(color)
                if (this.IsShowTwoLevelMenu == true)
                    localStorage.setItem('classicalLayout', "1");
                if (getPortalConfigByKey("IsClassicalLayout", true) == false)
                    localStorage.setItem('classicalLayout', "1");
                this.classicalLayout = parseInt(localStorage.getItem('classicalLayout')) === 1
                this.updateLayout()
            },
            closeDropdown: function (e) {
                try {
                    e.target.parentNode.parentNode.classList.remove('layui-show')

                } catch (e) {
                }
                var _this = this
                if (_this.closeTimeout) {
                    clearTimeout(_this.closeTimeout)
                    _this.closeTimeout = null
                }
                _this.closeTimeout = setTimeout(function () {
                    _this.showShortCut = false
                    _this.showShortOA = false
                    _this.showUserActions = false
                    _this.tabDropdownVisible = false
                    clearTimeout(_this.closeTimeout)
                    _this.closeTimeout = null
                }, 30)

            },
            stopTimeout: function () {
                clearTimeout(this.closeTimeout)
                this.closeTimeout = null
            },
            logout: function () {

                if (confirm("提示确定需要退出？") == false)
                    return;

                var handler = new HttpHandler("BP.WF.HttpHandler.WF_Portal");
                var data = handler.DoMethodReturnString("Default_LogOut");
                if (data.indexOf("err@") == 0) {
                    SetHref("./Login.htm?DoType=Logout&SystemNo=CCFast");
                }
                else {
                    SetHref(data); // "Login.htm?DoType=Logout";
                }
                

                //  win
                //  var url = getPortalConfigByKey("LogoutPath", "./") + data;
                //window.location.href = filterXSS(url);// "Login.htm?DoType=Logout";
            },
            logoutExt: function () {
                var handler = new HttpHandler("BP.WF.HttpHandler.WF_Portal");
                var data = handler.DoMethodReturnString("Default_LogOut");
                var url = getPortalConfigByKey("LogoutPath", "/Portal/Standard/") + data;
                window.top.location.href = url
                // SetHref(url); 
            },
            GoToAppClassic: function () {
                var webUser = new WebUser();
                SetHref(basePath + "/WF/AppClassic/Home.htm?UserNo=" + webUser.No + "&Token=" + webUser.Token + "&OrgNo=" + webUser.OrgNo);
            },
            GoToMobile: function () {
                var webUser = new WebUser();  //退出
                var url = basePath + "/FastMobilePortal/Login.htm?UserNo=" + webUser.No + "&Token=" + webUser.Token + "&OrgNo=" + webUser.OrgNo;
                SetHref(url);
            },
            initMenus: function () {

                var handler = new HttpHandler("BP.WF.HttpHandler.WF_Portal");
                var data = handler.DoMethodReturnString("Default_Init");
                if (data.indexOf('err@') == 0) {
                    alert(data);
                    return;
                }
                if (data.indexOf('url@') == 0) {
                    var url = data.replace('url@', '');
                    SetHref(url);
                    return;
                }
                data = JSON.parse(data);
                this.refreshMenuTree(data);
                this.initContextMenu()
            },
            initContextMenu: function () {
            },
            initChildContextMenu: function () {

            },
            bindDropdown: function (type) {

            },

            openLayer: function (uri, name, w, h) {
                //console.log(uri, name);

                if (w === 0)
                    w = window.innerWidth;

                if (w === undefined)
                    w = window.innerWidth / 2;

                if (h === undefined)
                    h = window.innerHeight;

                layer.open({
                    type: 2,
                    title: name,
                    content: [uri, 'no'],
                    area: [w + 'px', h + 'px'],
                    offset: 'rb',
                    shadeClose: true
                })
            },
            calcClassList: function (item, type) {
                var cList = ['context-menu']
                if (item.type === 'flow') cList.push(type === 1 ? 'flow-node' : 'flow-node-child')
                if (item.type === 'form') cList.push(type === 1 ? 'form-node' : 'form-node-child')
                return cList
            },
            startListenMsg: function () {
                var _this = this
                if (this.msgListener) {
                    this.stopListenMsg()
                }
                this.msgListener = setInterval(() => {
                    var storedNotifyVal = window.localStorage.getItem("lastNotifyTime") || '0';
                    var lastNotifyTime = new Date(storedNotifyVal.replace(/-/g, '/')).getTime();

                    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
                    var data = handler.DoMethodReturnString("GenerMsg_Init");
                    if (data.indexOf('err@') > -1) {
                        alert(data);
                        return
                    }
                    try {
                        var json = JSON.parse(data)
                        var targetTimeMillions = new Date((json.ID + '').replace(/-/g, '/')).getTime();
                        if (json.Num > 0 && targetTimeMillions > lastNotifyTime ) {
                            document.getElementById("alert-msg").play();
                            layer.confirm("您有【" + json.Num + "】个新工作。", {
                                btn: ["去处理", "忽略"],
                            }, function () {
                                _this.openTab("待办", basePath+"/WF/Todolist.htm", false);
                                layer.close(layer.index);
                            }, function () {
                                layer.close(layer.index);
                            })
                            window.localStorage.setItem("lastNotifyTime", json.ID);
                        }
                    } catch (e) {
                        return
                    }
                }, 5000)
            },

            stopListenMsg: function () {
                clearInterval(this.msgListener)
            },
            ChangeLang: function () {
                //初演示
                layui.dropdown.render({
                    elem: '#lange'
                    , align: 'right'
                    , data: [{
                        title: '简体中文'
                        , id: 'zh-cn'
                    }, {
                        title: 'English'
                        , id: 'en-us'
                    }],
                    style:'top: 44px',
                     click: function (obj) {
                         localStorage.setItem("Lange", obj.id);
                         SetHref(GetHrefUrl());
                    }
                });
            },
            GetNameByLange: function (key) {
                return window.lang[key];
            }
        },
        mounted: function () {
            // fix firefox bug
            document.body.ondrop = function (event) {
                event.preventDefault();
                event.stopPropagation();
            }
            this.SystemName = getPortalConfigByKey("SystemName", "驰骋BPM");
            this.SystemLogo = getPortalConfigByKey("SystemLogo", "./image/logo.png");
            this.IsShowFast = getPortalConfigByKey("IsShowFast", true);
            this.IsShowOA = getPortalConfigByKey("IsShowOA", true);
            this.IsShowRefresh = getPortalConfigByKey("IsShowRefresh", true);
            this.IsShowFullScreen = getPortalConfigByKey("IsShowFullScreen", true);
            this.IsShowTheme = getPortalConfigByKey("IsShowTheme", true);
            this.IsShowFlexible = getPortalConfigByKey("IsShowFlexible", true);
            this.IsShowTwoLevelMenu = getPortalConfigByKey("IsShowTwoLevelMenu", false);
            this.webUser = new WebUser();
            this.isAdmin = this.webUser.No === "admin" || parseInt(this.webUser.IsAdmin) === 1;
            this.plant = plant;
            this.initMenus();
            this.startListenMsg();
            this.initIM();
            this.ChangeLang();
        },
        beforeDestory: function () {
            this.stopListenMsg()
        }
    })

}