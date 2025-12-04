angular.module('VirtoCommerce.EnvironmentsCompare')
    .controller('VirtoCommerce.EnvironmentsCompare.environmentsComparisonController',
        [
            '$scope',
            'VirtoCommerce.EnvironmentsCompare.webApi',
            'platformWebApp.uiGridHelper',
            function (
                $scope,
                environmentsCompareApi,
                uiGridHelper) {
                const blade = $scope.blade;

                blade.title = 'environments-compare.blades.environments-comparison.title';
                blade.filter = blade.filter || {};
                if (typeof blade.hideEmptySections === 'undefined') {
                    blade.hideEmptySections = true;
                }

                var filter = blade.filter;
                filter.keyword = filter.keyword || '';
                filter.criteriaChanged = filter.criteriaChanged || function () { };

                function recalculateWidths() {
                    const containerWidth = window.innerWidth || 1200;
                    blade.markerColWidth = 5;
                    blade.nameColWidth = Math.min(600, Math.max(300, containerWidth * 0.3));
                    const envCount = blade.environmentNames && blade.environmentNames.length ? blade.environmentNames.length : 1;
                    const remaining = containerWidth - blade.markerColWidth - blade.nameColWidth;
                    blade.valueColWidth = Math.max(180, Math.floor(remaining / envCount));
                    blade.totalWidth = blade.markerColWidth + blade.nameColWidth + blade.valueColWidth * envCount;
                }

                recalculateWidths();

                window.addEventListener('resize', recalculateWidths);

                blade.refresh = function () {
                    blade.isLoading = true;

                    environmentsCompareApi.compareEnvironments(
                        {
                            environmentNames: blade.environmentNames,
                            baseEnvironmentName: blade.baseEnvironmentName,
                            showAll: blade.showAll
                        },
                        function (compareEnvironmentsResult) {
                            blade.data = compareEnvironmentsResult;
                            blade.isLoading = false;
                        });
                };

                blade.setBaseEnvironment = function (environment) {
                    if (environment.isComparisonBase === true) {
                        return;
                    }

                    blade.baseEnvironmentName = environment.environmentName;
                    blade.refresh();
                }

                blade.showAllSettings = function () {
                    blade.showAll = true;
                    blade.refresh();
                }

                blade.showDiffSettings = function () {
                    blade.showAll = false;
                    blade.refresh();
                }

                blade.getComparedValueOrError = function (settings, environmentName) {
                    if (!settings || !Array.isArray(settings.comparedValues)) {
                        return '';
                    }

                    const item = settings.comparedValues.find(x => x.environmentName === environmentName);
                    if (!item) {
                        return '';
                    }

                    return item.errorMessage || item.value;
                }

                blade.environmentComparedValueHasDiff = function (settings, environmentName) {
                    if (!settings || !Array.isArray(settings.comparedValues)) {
                        return false;
                    }

                    const comparedValue = settings.comparedValues.find(x => x.environmentName === environmentName);
                    return comparedValue ? blade.comparedValueHasDiff(comparedValue) : false;
                }

                blade.environmentComparedValueHasError = function (settings, environmentName) {
                    if (!settings || !Array.isArray(settings.comparedValues)) {
                        return false;
                    }

                    const comparedValue = settings.comparedValues.find(x => x.environmentName === environmentName);
                    return comparedValue ? blade.comparedValueHasError(comparedValue) : false;
                }

                blade.anyEnvironmentComparedValueHasDiff = function (settings) {
                    if (!settings || !Array.isArray(settings.comparedValues)) {
                        return false;
                    }

                    return settings.comparedValues.some(x => blade.comparedValueHasDiff(x));
                }

                blade.comparedValueHasDiff = function (item) {
                    if (!item) {
                        return false;
                    }

                    return item.equalsBaseValue === false && !item.errorMessage;
                }

                blade.comparedValueHasError = function (item) {
                    return !!(item && item.errorMessage);
                }

                blade.filterByText = function (setting) {
                    if (!blade.filter || !blade.filter.keyword) {
                        return true;
                    }

                    const term = blade.filter.keyword.toLowerCase();
                    return setting && setting.name && setting.name.toLowerCase().indexOf(term) !== -1;
                }

                blade.groupHasVisibleSettings = function (settingGroup) {
                    if (!settingGroup || !Array.isArray(settingGroup.settings)) {
                        return false;
                    }

                    return settingGroup.settings.some(blade.filterByText);
                }

                blade.scopeHasVisibleSettings = function (settingScope) {
                    if (!settingScope || !Array.isArray(settingScope.settingGroups)) {
                        return false;
                    }

                    return settingScope.settingGroups.some(blade.groupHasVisibleSettings);
                }

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
                            name: 'environments-compare.blades.environments-comparison.toolbar.show-all',
                            icon: 'fas fa-equals',
                            executeMethod: function () {
                                blade.showAllSettings();
                            },
                            hide: function () {
                                return blade.showAll === true;
                            },
                            canExecuteMethod: function () {
                                return true;
                            }
                        },
                        {
                            name: 'environments-compare.blades.environments-comparison.toolbar.show-diff',
                            icon: 'fas fa-not-equal',
                            executeMethod: function () {
                                blade.showDiffSettings();
                            },
                            hide: function () {
                                return !blade.showAll;
                            },
                            canExecuteMethod: function () {
                                return true;
                            }
                        }
                    ];
                }

                blade.refresh();
                $scope.$on('$destroy', function () {
                    window.removeEventListener('resize', recalculateWidths);
                });
                initializeToolbar();
            }]);