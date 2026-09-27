/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                   Component : Domain Layer                          *
*  Assembly : FinancialAccounting.AccountingRules.dll      Pattern   : Information holder aggregate          *
*  Type     : AccountingRule                               License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Holds an accounting guide rule and its rule entries aggregate.                                 *
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

  /// <summary>Holds an accounting guide rule and its rule entries aggregate.</summary>
  public sealed class AccountingRule : BaseObject {

    #region Fields

    private Lazy<List<AccountingRuleEntry>> _entries;

    #endregion Fields

    #region Constructors and parsers

    private AccountingRule() {
      // Required by Empiria Framework.
    }

    static public AccountingRule Parse(int id) => ParseId<AccountingRule>(id);

    static public AccountingRule Parse(string uid) => ParseKey<AccountingRule>(uid);

    static public AccountingRule Empty => ParseEmpty<AccountingRule>();

    protected override void OnLoad() {
      _entries = new Lazy<List<AccountingRuleEntry>>(() => AccountingRulesData.GetAccountingRuleEntries(this));
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("ACR_GUIDE_ID")]
    public AccountingGuide Guide {
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
        if (string.IsNullOrWhiteSpace(_selectorValues)) {
          return new FixedList<string>();
        }

        return _selectorValues.Split(',')
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


    public FixedList<AccountingRuleEntry> Entries {
      get {
        return _entries.Value.ToFixedList();
      }
    }

    #endregion Properties

  }  // class AccountingRule

}  // namespace Empiria.FinancialAccounting.AccountingRules
