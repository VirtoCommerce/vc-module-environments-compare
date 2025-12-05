angular.module('VirtoCommerce.EnvironmentsCompare')
    .controller('VirtoCommerce.EnvironmentsCompare.environmentItemController',
        [
            '$scope',
            'VirtoCommerce.EnvironmentsCompare.webApi',
            function (
                $scope,
                environmentsCompareApi) {
                const blade = $scope.blade;

                blade.title = blade.environmentName || 'environments-compare.blades.environments-comparison.title';
                blade.filter = blade.filter || {};
                if (typeof blade.hideEmptySections === 'undefined') {
                    blade.hideEmptySections = true;
                }

                var filter = blade.filter;
                filter.keyword = filter.keyword || '';
                filter.criteriaChanged = filter.criteriaChanged || function () { };

                blade.settingScopes = [];

                blade.refresh = function () {
                    blade.isLoading = true;

                    environmentsCompareApi.getEnvironmentSettings(
                        { environmentName: blade.environmentName },
                        function (result) {
                            blade.envSettings = result;
                            blade.settingScopes = result && result.settingScopes ? result.settingScopes : [];
                            blade.isLoading = false;
                        },
                        function () {
                            blade.isLoading = false;
                        });
                };

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

                blade.refresh();
            }]);


