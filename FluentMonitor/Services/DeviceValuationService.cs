using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentMonitor.Services
{
    // Device Recommendation Models
    public class DeviceRecommendation
    {
        public string DeviceType { get; set; } = string.Empty; // CPU, GPU, Memory, Storage
        public string DeviceName { get; set; } = string.Empty;
        public DeviceAge Age { get; set; }
        public int EstimatedValue { get; set; } // 估值（元）
        public int RecycleValue { get; set; } // 回收价（元）
        public RepairOption? OfficialRepair { get; set; }
        public RepairOption? MarketRepair { get; set; }
        public UpgradeRecommendation? UpgradeOption { get; set; }
        public string Recommendation { get; set; } = string.Empty;
        public RecommendationLevel Level { get; set; }
    }

    public class RepairOption
    {
        public string Provider { get; set; } = string.Empty; // "官方" or "商城"
        public int RepairCost { get; set; } // 维修费用
        public int ReplacementCost { get; set; } // 换新费用
        public string Note { get; set; } = string.Empty;
    }

    public class UpgradeRecommendation
    {
        public string RecommendedModel { get; set; } = string.Empty;
        public int EstimatedCost { get; set; }
        public string Benefit { get; set; } = string.Empty;
    }

    public enum DeviceAge
    {
        New,        // < 1 year
        Recent,     // 1-2 years
        Moderate,   // 2-4 years
        Old,        // 4-6 years
        VeryOld,    // 6-8 years
        Obsolete    // > 8 years
    }

    public enum RecommendationLevel
    {
        Excellent,  // 设备状态优秀，无需操作
        Good,       // 设备良好，继续使用
        Consider,   // 考虑升级
        Recommend,  // 推荐升级
        Urgent      // 强烈建议升级/更换
    }

    public class DeviceValuationService
    {
        // 评估 CPU 设备
        public DeviceRecommendation? EvaluateCpu(string cpuName, int? releaseYear = null)
        {
            if (string.IsNullOrEmpty(cpuName))
                return null;

            var age = EstimateCpuAge(cpuName, releaseYear);
            var recommendation = new DeviceRecommendation
            {
                DeviceType = "处理器 (CPU)",
                DeviceName = cpuName,
                Age = age
            };

            // 根据年代评估
            switch (age)
            {
                case DeviceAge.New:
                case DeviceAge.Recent:
                    recommendation.Level = RecommendationLevel.Excellent;
                    recommendation.Recommendation = "设备性能优秀，无需升级";
                    recommendation.EstimatedValue = EstimateCpuValue(cpuName, age);
                    recommendation.RecycleValue = 0;
                    break;

                case DeviceAge.Moderate:
                    recommendation.Level = RecommendationLevel.Good;
                    recommendation.Recommendation = "设备仍可满足日常使用，可继续使用 1-2 年";
                    recommendation.EstimatedValue = EstimateCpuValue(cpuName, age);
                    recommendation.RecycleValue = (int)(recommendation.EstimatedValue * 0.4);
                    break;

                case DeviceAge.Old:
                    recommendation.Level = RecommendationLevel.Consider;
                    recommendation.Recommendation = "设备性能开始落后，建议考虑升级以获得更好体验";
                    recommendation.EstimatedValue = EstimateCpuValue(cpuName, age);
                    recommendation.RecycleValue = (int)(recommendation.EstimatedValue * 0.3);
                    recommendation.OfficialRepair = new RepairOption
                    {
                        Provider = "官方售后",
                        RepairCost = 500,
                        ReplacementCost = 2000,
                        Note = "官方质保，价格较高"
                    };
                    recommendation.MarketRepair = new RepairOption
                    {
                        Provider = "第三方维修",
                        RepairCost = 200,
                        ReplacementCost = 800,
                        Note = "性价比高，质保期短"
                    };
                    recommendation.UpgradeOption = new UpgradeRecommendation
                    {
                        RecommendedModel = GetRecommendedCpu(cpuName),
                        EstimatedCost = 1500,
                        Benefit = "性能提升 2-3 倍，支持最新技术"
                    };
                    break;

                case DeviceAge.VeryOld:
                    recommendation.Level = RecommendationLevel.Recommend;
                    recommendation.Recommendation = "设备严重老化，推荐升级以提升性能和安全性";
                    recommendation.EstimatedValue = EstimateCpuValue(cpuName, age);
                    recommendation.RecycleValue = (int)(recommendation.EstimatedValue * 0.2);
                    recommendation.OfficialRepair = new RepairOption
                    {
                        Provider = "官方售后",
                        RepairCost = 800,
                        ReplacementCost = 3000,
                        Note = "不推荐维修，建议直接换新"
                    };
                    recommendation.MarketRepair = new RepairOption
                    {
                        Provider = "第三方维修",
                        RepairCost = 300,
                        ReplacementCost = 1200,
                        Note = "维修价值较低，推荐以旧换新"
                    };
                    recommendation.UpgradeOption = new UpgradeRecommendation
                    {
                        RecommendedModel = GetRecommendedCpu(cpuName),
                        EstimatedCost = 2000,
                        Benefit = "性能提升 5-8 倍，能耗降低 40%"
                    };
                    break;

                case DeviceAge.Obsolete:
                    recommendation.Level = RecommendationLevel.Urgent;
                    recommendation.Recommendation = "设备已过时，强烈建议立即更换以确保安全和性能";
                    recommendation.EstimatedValue = 0;
                    recommendation.RecycleValue = 50; // 仅回收价值
                    recommendation.OfficialRepair = new RepairOption
                    {
                        Provider = "官方售后",
                        RepairCost = 0,
                        ReplacementCost = 0,
                        Note = "已停止售后服务"
                    };
                    recommendation.MarketRepair = new RepairOption
                    {
                        Provider = "第三方回收",
                        RepairCost = 0,
                        ReplacementCost = 0,
                        Note = "建议回收，环保处理"
                    };
                    recommendation.UpgradeOption = new UpgradeRecommendation
                    {
                        RecommendedModel = GetRecommendedCpu(cpuName),
                        EstimatedCost = 2500,
                        Benefit = "性能提升 10+ 倍，支持最新应用"
                    };
                    break;
            }

            return recommendation;
        }

        // 评估内存设备
        public DeviceRecommendation? EvaluateMemory(int speedMHz, int capacityGB)
        {
            var age = EstimateMemoryAge(speedMHz);
            var recommendation = new DeviceRecommendation
            {
                DeviceType = "内存 (RAM)",
                DeviceName = $"{capacityGB}GB DDR{GetDdrGeneration(speedMHz)} @ {speedMHz}MHz",
                Age = age
            };

            switch (age)
            {
                case DeviceAge.New:
                case DeviceAge.Recent:
                    recommendation.Level = RecommendationLevel.Excellent;
                    recommendation.Recommendation = capacityGB >= 16 ? "内存容量和速度优秀" : "建议扩容至 16GB 以获得更好性能";
                    recommendation.EstimatedValue = capacityGB * 100;
                    break;

                case DeviceAge.Moderate:
                    recommendation.Level = RecommendationLevel.Good;
                    recommendation.Recommendation = "内存仍可满足日常使用";
                    recommendation.EstimatedValue = capacityGB * 60;
                    recommendation.RecycleValue = capacityGB * 20;
                    break;

                case DeviceAge.Old:
                case DeviceAge.VeryOld:
                    recommendation.Level = RecommendationLevel.Recommend;
                    recommendation.Recommendation = "内存代数较旧，建议升级至 DDR4/DDR5";
                    recommendation.EstimatedValue = capacityGB * 30;
                    recommendation.RecycleValue = capacityGB * 10;
                    recommendation.UpgradeOption = new UpgradeRecommendation
                    {
                        RecommendedModel = $"32GB DDR5 @ 5600MHz",
                        EstimatedCost = 800,
                        Benefit = "速度提升 2-3 倍，容量翻倍"
                    };
                    break;

                case DeviceAge.Obsolete:
                    recommendation.Level = RecommendationLevel.Urgent;
                    recommendation.Recommendation = "内存已过时，强烈建议更换";
                    recommendation.EstimatedValue = 0;
                    recommendation.RecycleValue = 20;
                    recommendation.UpgradeOption = new UpgradeRecommendation
                    {
                        RecommendedModel = $"32GB DDR5 @ 5600MHz",
                        EstimatedCost = 800,
                        Benefit = "速度提升 5+ 倍，支持最新平台"
                    };
                    break;
            }

            return recommendation;
        }

        // 估算 CPU 年代
        private DeviceAge EstimateCpuAge(string cpuName, int? releaseYear)
        {
            if (releaseYear.HasValue)
            {
                int yearsOld = DateTime.Now.Year - releaseYear.Value;
                if (yearsOld < 1) return DeviceAge.New;
                if (yearsOld < 2) return DeviceAge.Recent;
                if (yearsOld < 4) return DeviceAge.Moderate;
                if (yearsOld < 6) return DeviceAge.Old;
                if (yearsOld < 8) return DeviceAge.VeryOld;
                return DeviceAge.Obsolete;
            }

            // 基于型号名称推断
            var name = cpuName.ToLower();

            // Intel 代数
            if (name.Contains("core"))
            {
                // 14th gen (2023-2024)
                if (name.Contains("14900") || name.Contains("14700") || name.Contains("14600"))
                    return DeviceAge.New;

                // 13th gen (2022-2023)
                if (name.Contains("13900") || name.Contains("13700") || name.Contains("13600"))
                    return DeviceAge.Recent;

                // 12th gen (2021-2022)
                if (name.Contains("12900") || name.Contains("12700") || name.Contains("12600"))
                    return DeviceAge.Moderate;

                // 11th gen (2020-2021)
                if (name.Contains("11900") || name.Contains("11700") || name.Contains("11600"))
                    return DeviceAge.Moderate;

                // 10th gen (2019-2020)
                if (name.Contains("10900") || name.Contains("10700") || name.Contains("10600"))
                    return DeviceAge.Old;

                // 9th gen and older
                if (name.Contains("9900") || name.Contains("9700") || name.Contains("8700") || name.Contains("7700"))
                    return DeviceAge.VeryOld;

                // 6th gen and older
                return DeviceAge.Obsolete;
            }

            // AMD Ryzen
            if (name.Contains("ryzen"))
            {
                // Ryzen 7000 series (2022-2024)
                if (name.Contains("7950") || name.Contains("7900") || name.Contains("7700") || name.Contains("7600"))
                    return DeviceAge.New;

                // Ryzen 5000 series (2020-2022)
                if (name.Contains("5950") || name.Contains("5900") || name.Contains("5800") || name.Contains("5600"))
                    return DeviceAge.Recent;

                // Ryzen 3000 series (2019-2020)
                if (name.Contains("3950") || name.Contains("3900") || name.Contains("3700") || name.Contains("3600"))
                    return DeviceAge.Moderate;

                // Ryzen 2000 series (2018-2019)
                if (name.Contains("2700") || name.Contains("2600"))
                    return DeviceAge.Old;

                // Ryzen 1000 series (2017-2018)
                if (name.Contains("1800") || name.Contains("1700") || name.Contains("1600"))
                    return DeviceAge.VeryOld;

                return DeviceAge.Obsolete;
            }

            // 默认判断为较旧
            return DeviceAge.Old;
        }

        // 估算内存年代
        private DeviceAge EstimateMemoryAge(int speedMHz)
        {
            // DDR5 (2021+)
            if (speedMHz >= 4800)
                return DeviceAge.New;

            // DDR4 高频 (2018-2021)
            if (speedMHz >= 3200)
                return DeviceAge.Recent;

            // DDR4 中频 (2015-2018)
            if (speedMHz >= 2400)
                return DeviceAge.Moderate;

            // DDR4 低频 / DDR3 高频 (2012-2015)
            if (speedMHz >= 1600)
                return DeviceAge.Old;

            // DDR3 及更早
            if (speedMHz >= 800)
                return DeviceAge.VeryOld;

            return DeviceAge.Obsolete;
        }

        // 估算 CPU 价值
        private int EstimateCpuValue(string cpuName, DeviceAge age)
        {
            // 基础价值
            int baseValue = 2000;

            // 高端型号
            if (cpuName.ToLower().Contains("9900") || cpuName.ToLower().Contains("7950") ||
                cpuName.ToLower().Contains("13900") || cpuName.ToLower().Contains("14900"))
            {
                baseValue = 4000;
            }
            else if (cpuName.ToLower().Contains("9700") || cpuName.ToLower().Contains("7900") ||
                     cpuName.ToLower().Contains("13700") || cpuName.ToLower().Contains("14700"))
            {
                baseValue = 3000;
            }

            // 根据年代折旧
            return age switch
            {
                DeviceAge.New => baseValue,
                DeviceAge.Recent => (int)(baseValue * 0.8),
                DeviceAge.Moderate => (int)(baseValue * 0.5),
                DeviceAge.Old => (int)(baseValue * 0.3),
                DeviceAge.VeryOld => (int)(baseValue * 0.15),
                DeviceAge.Obsolete => 0,
                _ => baseValue
            };
        }

        // 推荐 CPU 型号
        private string GetRecommendedCpu(string currentCpu)
        {
            var name = currentCpu.ToLower();

            // Intel 用户推荐 Intel
            if (name.Contains("intel") || name.Contains("core"))
            {
                return "Intel Core i7-14700K (第14代)";
            }

            // AMD 用户推荐 AMD
            if (name.Contains("amd") || name.Contains("ryzen"))
            {
                return "AMD Ryzen 7 7800X3D";
            }

            // 默认推荐性价比型号
            return "Intel Core i5-14600K 或 AMD Ryzen 5 7600X";
        }

        // 获取 DDR 代数
        private string GetDdrGeneration(int speedMHz)
        {
            if (speedMHz >= 4800) return "5";
            if (speedMHz >= 2133) return "4";
            if (speedMHz >= 800) return "3";
            return "2";
        }

        // 获取年代颜色
        public string GetAgeColor(DeviceAge age)
        {
            return age switch
            {
                DeviceAge.New => "#10893E",           // Green
                DeviceAge.Recent => "#0078D4",        // Blue
                DeviceAge.Moderate => "#00CC66",      // Light Green
                DeviceAge.Old => "#FF8C00",           // Orange
                DeviceAge.VeryOld => "#E81123",       // Red
                DeviceAge.Obsolete => "#808080",      // Gray
                _ => "#808080"
            };
        }

        // 获取推荐等级颜色
        public string GetRecommendationLevelColor(RecommendationLevel level)
        {
            return level switch
            {
                RecommendationLevel.Excellent => "#10893E",
                RecommendationLevel.Good => "#0078D4",
                RecommendationLevel.Consider => "#00CC66",
                RecommendationLevel.Recommend => "#FF8C00",
                RecommendationLevel.Urgent => "#E81123",
                _ => "#808080"
            };
        }

        // 获取年代描述
        public string GetAgeDescription(DeviceAge age)
        {
            return age switch
            {
                DeviceAge.New => "全新 (< 1年)",
                DeviceAge.Recent => "较新 (1-2年)",
                DeviceAge.Moderate => "中等 (2-4年)",
                DeviceAge.Old => "较旧 (4-6年)",
                DeviceAge.VeryOld => "很旧 (6-8年)",
                DeviceAge.Obsolete => "过时 (> 8年)",
                _ => "未知"
            };
        }
    }
}
