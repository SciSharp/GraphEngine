﻿// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Trinity.Diagnostics;
using Trinity.Storage;

namespace Trinity.Modules.Spark
{
    public interface ISparkTrinityConnector
    {
        StructType GetSchema(string jsonstr);

        object GetPartitions(string jsonstr);

        IEnumerable<object> GetPartition(string jsonstr);
    }

    public class DefaultSparkTrinityConnector : ISparkTrinityConnector
    {

        public ICellRepository CellRepository { get; set; } = new DefaultCellRepository();

        public StructType GetSchema(string jsonstr)
        {
            JObject json;
            string cellType;
            if (!Utilities.TryDeserializeObject(jsonstr, out json) ||
                !Utilities.TryGetValue(json, "cellType", out cellType))
            {
                return null;
            }

            return CellRepository.GetCellSchema(cellType);
        }

        public object GetPartitions(string jsonstr)
        {
            JObject json;
            string cellType;
            int batchSize;
            if (!Utilities.TryDeserializeObject(jsonstr, out json) ||
                !Utilities.TryGetValue(json, "cellType", out cellType) ||
                !Utilities.TryGetValue(json, "batchSize", out batchSize))
            {
                return null;
            }

            var partitions = new List<List<long>>();

            if (batchSize <= 0)
                return partitions;

            List<JObject> filters = null;
            Utilities.TryGetList(json, "filters", out filters);

            var cellIds = CellRepository.FindCells(cellType, filters);
            var part = new List<long>();
            var count = 0;
            foreach (var id in cellIds)
            {
                part.Add(id);
                count++;
                if (count % batchSize == 0)
                {
                    partitions.Add(part);
                    part = new List<long>();
                }
            }

            if (part.Count() > 0)
                partitions.Add(part);

            return partitions;
        }

        public IEnumerable<object> GetPartition(string jsonstr)
        {
            JObject json;
            string cellType;
            List<long> cellIds;
            if (!Utilities.TryDeserializeObject(jsonstr, out json) ||
                !Utilities.TryGetValue(json, "cellType", out cellType) ||
                !Utilities.TryGetList(json, "partition", out cellIds))
            {
                return null;
            }

            var cellDesc = Global.StorageSchema.CellDescriptors.FirstOrDefault(_ => _.TypeName == cellType);
            if (cellDesc == null)
                return null;

            List<string> fieldNames = null;
            Utilities.TryGetList(json, "fields", out fieldNames);

            var timer = Stopwatch.StartNew();
            var cells = cellIds.Select(id => CellRepository.LoadCell(cellType, id, fieldNames));
            timer.Stop();
            Log.WriteLine(LogLevel.Info, $"GetPartition[cellType={cellType}, cellIds={cellIds.Count()}] succeeded: count={cells.Count()}, timer={(long)timer.Elapsed.TotalMilliseconds}");

            return cells;
        }
    }
}
