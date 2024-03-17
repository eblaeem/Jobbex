System.register([], function (exports_1, context_1) {
    "use strict";
    var Home;
    var __moduleName = context_1 && context_1.id;
    return {
        setters: [],
        execute: function () {
            Home = (function () {
                function Home() {
                    $('#active-jobs').mCustomScrollbar({
                        theme: "dark"
                    });
                }
                return Home;
            }());
            exports_1("Home", Home);
            new Home();
        }
    };
});
//# sourceMappingURL=home.js.map