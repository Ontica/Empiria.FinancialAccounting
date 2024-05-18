/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : External Data                              Component : Domain Layer                            *
*  Assembly : FinancialAccounting.ExternalData.dll       Pattern   : Empiria Data Object                     *
*  Type     : ExternalVariable                           License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Defines an external variable like a financial indicator or business income.                    *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Contacts;
using Empiria.DynamicData;
using Empiria.Json;
using Empiria.StateEnums;

using Empiria.FinancialAccounting.ExternalData.Adapters;
using Empiria.FinancialAccounting.ExternalData.Data;
using Empiria.FinancialAccounting.Datasets;

namespace Empiria.FinancialAccounting.ExternalData {

  /// <summary>Holds data about an external variable or value like a
  /// financial indicator or business income.</summary>
  public class ExternalValue : BaseObject {

    #region Constructors and parsers

    protected ExternalValue() {
      // Required by Empiria Framework.
    }

    public ExternalValue(ExternalValueInputDto dto) {
      Assertion.Require(dto, nameof(dto));

      this.Load(dto);
    }


    static public ExternalValue Parse(int id) {
      return BaseObject.ParseId<ExternalValue>(id);
    }


    static public ExternalValue Empty {
      get {
        return ExternalValue.ParseEmpty<ExternalValue>();
      }
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("ID_VARIABLE_EXTERNA")]
    public ExternalVariable ExternalVariable {
      get;
      private set;
    }

    [DataField("VALORES_VARIABLE")]
    internal JsonObject ValuesExtData {
      get;
      private set;
    } = new JsonObject();


    [DataField("FECHA_APLICACION")]
    public DateTime ApplicationDate {
      get;
      private set;
    }


    [DataField("ID_EDITADO_POR")]
    public Contact UpdatedBy {
      get;
      private set;
    }


    [DataField("FECHA_EDICION")]
    public DateTime UpdatedDate {
      get;
      private set;
    }


    [DataField("ID_ARCHIVO")]
    public Dataset SourceDataset {
      get;
      private set;
    }


    [DataField("STATUS_VALOR_EXTERNO", Default = EntityStatus.Active)]
    public EntityStatus Status {
      get;
      private set;
    }

    #endregion Properties

    internal void Delete() {
      this.Status = EntityStatus.Deleted;
    }


    private void Load(ExternalValueInputDto dto) {
      this.ExternalVariable = ExternalVariable.Parse(dto.VariableUID);
      this.ValuesExtData = dto.GetDynamicFieldsAsJson();
      this.ApplicationDate = dto.ApplicationDate;
      this.SourceDataset = dto.Dataset;
      this.UpdatedBy = dto.UpdatedBy;
      this.UpdatedDate = dto.UpdatedDate;
      this.Status = dto.Status;
    }


    protected override void OnSave() {
      ExternalValuesData.Write(this);
    }


    public decimal GetTotalField(string fieldName) {
      return ValuesExtData.Get<decimal>(fieldName, 0m);
    }


    public DynamicFields GetDynamicFields() {
      var rawValues = this.ValuesExtData.ToDictionary();

      var fieldNames = this.ExternalVariable.Set.DataColumns.FindAll(x => x.Type == "decimal")
                                                            .Select(x => x.Field);

      var fields = new DynamicFields();

      foreach (var fieldName in fieldNames) {
        if (rawValues.ContainsKey(fieldName)) {
          fields.SetTotalField(fieldName, Convert.ToDecimal(rawValues[fieldName]));
        }
      }

      return fields;
    }

  } // class ExternalValue

}  // namespace Empiria.FinancialAccounting.ExternalData
