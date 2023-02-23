/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting                       Component : Common Types                            *
*  Assembly : FinancialAccounting.Core.dll               Pattern   : Information Holder                      *
*  Types    : ExportTo and ExporToDto                    License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Information holder with file exportation rules.                                                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Json;

namespace Empiria.FinancialAccounting {

  /// <summary>DTO version of ExportTo type.</summary>
  public class ExportToDto {

    internal ExportToDto() {
      // no-op
    }

    public string UID {
      get; internal set;
    }

    public string Name {
      get; internal set;
    }

    public string FileType {
      get; internal set;
    }

    public string Dataset {
      get; internal set;
    }

    public DateTime StartDate {
      get; internal set;
    }

    public DateTime EndDate {
      get; internal set;
    }

  }  // class ExportToDto



  /// <summary>Information holder with file exportation rules.</summary>
  public class ExportTo {

    private ExportTo() {
      // no-op
    }


    static internal ExportTo Parse(JsonObject json) {
      return new ExportTo {
        Name = json.Get<string>("name"),
        FileType = json.Get<string>("fileType"),
        FileName = json.Get<string>("fileName", string.Empty),
        CsvBuilder = json.Get<string>("csvBuilder", string.Empty),
        TemplateId = json.Get<int>("templateId", -1),
        Dataset = json.Get<string>("dataset", "Default"),
        StartDate = json.Get<DateTime>("startDate", AccountsChart.IFRS.MasterData.StartDate),
        EndDate = json.Get<DateTime>("endDate", AccountsChart.IFRS.MasterData.EndDate),
      };
    }

    public string UID {
      get {
        return $"{Name}.{Dataset}";
      }
    }

    public string CalculatedUID {
      get {
        if (TemplateId == -1) {
          return this.UID;
        }

        var template = FileTemplateConfig.Parse(TemplateId);

        return template.UID;
      }
    }


    public string Name {
      get; private set;
    }


    public string FileType {
      get; private set;
    }


    public string FileName {
      get; private set;
    }


    public string CsvBuilder {
      get; private set;
    }


    public int TemplateId {
      get; private set;
    }


    public string Dataset {
      get; private set;
    }

    public DateTime StartDate {
      get; private set;
    }

    public DateTime EndDate {
      get; private set;
    }

  }  // class ExportTo

}  // namespace Empiria.FinancialAccounting
