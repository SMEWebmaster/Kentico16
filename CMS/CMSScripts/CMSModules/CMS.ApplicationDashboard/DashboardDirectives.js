define(["require", "exports", 'angular', 'CMS.ApplicationDashboard/Directives.TileDirective', 'CMS.ApplicationDashboard/Directives.WelcomeTileDirective'], function(require, exports, angular, tileDirective, welcomeTileDirective) {
    var ModuleName = 'cms.dashboard.directives';
    
    var tileIconDirective = function () {
        return {
            restrict: 'A',
            scope: {
                iconAlternativeText: '= ',
                iconStyle: '@',
                icon: '=tileIcon',
            },
            templateUrl: 'tileIconTemplate.html'
        };
    };
    
    angular.module(ModuleName, []).directive('tile', tileDirective.Directive).directive('welcometile', welcomeTileDirective.Directive).directive('tileIcon', tileIconDirective);

    exports.Module = ModuleName;
});
