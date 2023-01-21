/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : External Data                              Component : Domain Layer                            *
*  Assembly : FinancialAccounting.ExternalData.dll       Pattern   : Empiria Aggregate Object                *
*  Type     : ExternalVariablesSet                       License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Holds a set of external variables, which the purpose of classify them.                         *
*             A given external variable always belongs to a single set.                                      *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.FinancialAccounting.Datasets;
using Empiria.FinancialAccounting.ExternalData.Adapters;
using Empiria.FinancialAccounting.ExternalData.Data;

namespace Empiria.FinancialAccounting.ExternalData {

  /// <summary>Holds a set of external variables, which the purpose of classify them.
  /// A given external variable always belongs to a single set.</summary>
  public class ExternalVariablesSet : DatasetFamily {

    #region Fields

    private Lazy<FixedList<ExternalVariable>> _externalVariables;

    #endregion Fields

    #region Constructors and parsers

    protected ExternalVariablesSet() {
      // Required by Empiria Framework.
    }


    static public new ExternalVariablesSet Parse(int id) {
      return BaseObject.ParseId<ExternalVariablesSet>(id);
    }


    static public new ExternalVariablesSet Parse(string uid) {
      return BaseObject.ParseKey<ExternalVariablesSet>(uid);
    }


    static public FixedList<ExternalVariablesSet> GetList() {
      return BaseObject.GetList<ExternalVariablesSet>(String.Empty, "ObjectName")
                       .ToFixedList();
    }


    static public ExternalVariablesSet Empty {
      get {
        return BaseObject.ParseEmpty<ExternalVariablesSet>();
      }
    }


    protected override void OnLoad() {
      if (this.IsEmptyInstance) {
        return;
      }

      _externalVariables =
            new Lazy<FixedList<ExternalVariable>>(() => ExternalVariablesData.GetExternalVariables(this));

    }

    #endregion Constructors and parsers

    #region Properties

    public FixedList<DataTableColumn> DataColumns {
      get {
        return base.ExtendedDataField.GetFixedList<DataTableColumn>("dataColumns");
      }
      private set {
        base.ExtendedDataField.Set("dataColumns", value);
      }
    }


    public FixedList<ExportTo> ExportTo {
      get {
        return base.ExtendedDataField.GetFixedList<ExportTo>("exportTo");
      }
      private set {
        base.ExtendedDataField.Set("exportTo", value);
      }
    }


    public FixedList<ExternalVariable> ExternalVariables {
      get {
        return _externalVariables.Value;
      }
    }

    #endregion Properties


    #region Methods

    internal ExternalVariable Add(ExternalVariableFields fields) {
      throw new NotImplementedException();
    }

    internal ExternalVariable GetVariable(string variableUID) {
      throw new NotImplementedException();
    }

    internal void Delete(ExternalVariable variable) {
      throw new NotImplementedException();
    }

    internal void Update(ExternalVariable variable, ExternalVariableFields fields) {
      throw new NotImplementedException();
    }

    #endregion Methods

  } // class ExternalVariablesSet

}  // namespace Empiria.FinancialAccounting.ExternalData
