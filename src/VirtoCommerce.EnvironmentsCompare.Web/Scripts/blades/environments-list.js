angular.module('VirtoCommerce.EnvironmentsCompare')
    .controller('VirtoCommerce.EnvironmentsCompare.environmentsListController',
        [
            '$scope',
            'VirtoCommerce.EnvironmentsCompare.webApi',
            'platformWebApp.bladeNavigationService', 'platformWebApp.uiGridHelper',
            function (
                $scope,
                environmentsCompareApi,
                bladeNavigationService, uiGridHelper) {
                const blade = $scope.blade;

                blade.title = 'environments-compare.blades.environments-list.title';

                blade.refresh = function () {
                    environmentsCompareApi.getEnvironments(function (getEnvironmentsResult) {
                        blade.data = getEnvironmentsResult;
                        blade.isLoading = false;
                    });
                };

                $scope.compare = function () {
                    const environmentNames = _.pluck($scope.gridApi.selection.getSelectedRows(), 'name');

                    const comparisonBlade = {
                        id: 'environments-comparison-blade',
                        controller: 'VirtoCommerce.EnvironmentsCompare.environmentsComparisonController',
                        template: 'Modules/$(VirtoCommerce.EnvironmentsCompare)/Scripts/blades/environments-comparison.html',
                        environmentNames: environmentNames,
                        baseEnvironmentName: environmentNames[0],
                        showAll: false,
                    };

                    bladeNavigationService.showBlade(comparisonBlade, blade);
                };

                $scope.setGridOptions = function (gridOptions) {
                    uiGridHelper.initialize($scope, gridOptions, function (gridApi) {
                        $scope.gridApi = gridApi;
                    });
                };

                function initializeToolbar() {
                    blade.toolbarCommands = [
                        {
                            name: 'platform.commands.refresh',
                            icon: 'fa fa-refresh',
                            executeMethod: blade.refresh,
                            canExecuteMethod: function () {
                                return true;
                            }
                        },
                        {
                            name: 'environments-compare.blades.environments-list.toolbar.compare',
                            icon: 'fas fa-microscope',
                            executeMethod: $scope.compare,
                            canExecuteMethod: hasTwoOrMoreSelectedItems,
                        },
                    ];
                }

                function hasTwoOrMoreSelectedItems() {
                    return $scope.gridApi?.selection?.getSelectedRows()?.length >= 2;
                }

                blade.refresh();
                initializeToolbar();
            }]);
