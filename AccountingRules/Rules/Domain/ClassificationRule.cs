/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                   Component : Domain Layer                          *
*  Assembly : FinancialAccounting.AccountingRules.dll      Pattern   : Information holder                    *
*  Type     : ClassificationRule                           License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Defines the classifications offered for one or more selectors.                                 *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using System.Collections.Generic;
using System.Linq;

using Empiria.Json;
using Empiria.Parties;
using Empiria.StateEnums;

using Empiria.FinancialAccounting.AccountingRules.Data;

namespace Empiria.FinancialAccounting.AccountingRules {

  /// <summary>Defines the classifications offered for one or more selectors.</summary>
  public sealed class ClassificationRule : BaseObject {

    private Lazy<List<ClassificationEntry>> _entries;

    private ClassificationRule() {
      // Required by Empiria Framework.
    }

    #region Constructors and parsers

    static public ClassificationRule Parse(int id) => ParseId<ClassificationRule>(id);

    static public ClassificationRule Parse(string uid) => ParseKey<ClassificationRule>(uid);

    static public ClassificationRule Empty => ParseEmpty<ClassificationRule>();

    protected override void OnLoad() {
      _entries = new Lazy<List<ClassificationEntry>>(() => ClassificationRulesData.GetEntries(this));
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("ACR_GUIDE_ID")]
    public ClassificationGuide Guide {
      get; private set;
    }


    [DataField("ACR_CODE")]
    public string Code {
      get; private set;
    }


    [DataField("ACR_NAME")]
    public string Name {
      get; private set;
    }


    [DataField("ACR_PRIORITY")]
    public int Priority {
      get; private set;
    }


    [DataField("ACR_SELECTOR_VALUES")]
    private string _selectorValues = string.Empty;


    public FixedList<string> SelectorValues {
      get {
        return (_selectorValues ?? string.Empty).Split(',')
                .Select(x => EmpiriaString.Clean(x))
                .Where(x => x.Length != 0)
                .ToFixedList();
      }
    }


    [DataField("ACR_CONDITIONS_EXT_DATA")]
    public JsonObject Conditions {
      get; private set;
    }


    [DataField("ACR_EXT_DATA")]
    public JsonObject ExtData {
      get; private set;
    }


    [DataField("ACR_START_DATE")]
    public DateTime StartDate {
      get; private set;
    }


    [DataField("ACR_END_DATE")]
    public DateTime EndDate {
      get; private set;
    }


    [DataField("ACR_POSTED_BY_ID")]
    public Party PostedBy {
      get; private set;
    }


    [DataField("ACR_STATUS", Default = EntityStatus.Active)]
    public EntityStatus Status {
      get; private set;
    }


    public FixedList<ClassificationEntry> Entries {
      get {
        return _entries.Value.ToFixedList();
      }
    }

    #endregion Properties

  }  // class ClassificationRule

}  // namespace Empiria.FinancialAccounting.AccountingRules
