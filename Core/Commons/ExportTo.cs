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

  }


  /// <summary>Information holder with file exportation rules.</summary>
  public class ExportTo {

    protected internal ExportTo() {
      // no-op
    }


    static internal ExportTo Parse(JsonObject json) {
      return new ExportTo {
        Name = json.Get<string>("name"),
        FileType = json.Get<string>("fileType"),
        FileName = json.Get<string>("fileName", string.Empty),
        CsvBuilder = json.Get<string>("csvBuilder", string.Empty),
        TemplateId = json.Get<int>("templateId", -1),
        Dataset = json.Get<string>("dataset", "Default")
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


    [DataField("Name")]
    public string Name {
      get; private set;
    }


    [DataField("FileType")]
    public string FileType {
      get; private set;
    }


    [DataField("FileName")]
    public string FileName {
      get; private set;
    }


    [DataField("CsvBuilder")]
    public string CsvBuilder {
      get; private set;
    }


    [DataField("TemplateId")]
    public int TemplateId {
      get; private set;
    }


    [DataField("Dataset")]
    public string Dataset {
      get; private set;
    }

  }  // class ExportTo

}  // namespace Empiria.FinancialAccounting
