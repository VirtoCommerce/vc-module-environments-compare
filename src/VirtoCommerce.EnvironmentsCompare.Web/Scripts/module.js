// Call this to register your module to main application
var moduleName = 'VirtoCommerce.EnvironmentsCompare';

if (AppDependencies !== undefined) {
    AppDependencies.push(moduleName);
}

angular.module(moduleName, [])
    .config(['$stateProvider',
        function ($stateProvider) {
            $stateProvider
                .state('workspace.EnvironmentsCompareState', {
                    url: '/environments-compare',
                    templateUrl: '$(Platform)/Scripts/common/templates/home.tpl.html',
                    controller: [
                        'platformWebApp.bladeNavigationService',
                        function (bladeNavigationService) {
                            var newBlade = {
                                id: 'environments-list-blade',
                                controller: 'VirtoCommerce.EnvironmentsCompare.environmentsListController',
                                template: 'Modules/$(VirtoCommerce.EnvironmentsCompare)/Scripts/blades/environments-list.html',
                                isClosingDisabled: true,
                            };
                            bladeNavigationService.showBlade(newBlade);
                        }
                    ]
                });
        }
    ])
    .run(['platformWebApp.mainMenuService', '$state',
        function (mainMenuService, $state) {
            //Register module in main menu
            var menuItem = {
                path: 'browse/environments-compare',
                icon: 'fa fa-cube',
                title: 'environments-compare.main-menu-title',
                priority: 100,
                action: function () { $state.go('workspace.EnvironmentsCompareState'); },
                permission: 'environments-compare:access',
            };
            mainMenuService.addMenuItem(menuItem);
        }
    ]);
