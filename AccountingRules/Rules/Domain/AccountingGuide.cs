/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                   Component : Domain Layer                          *
*  Assembly : FinancialAccounting.AccountingRules.dll      Pattern   : Aggregate root                        *
*  Type     : AccountingGuide                              License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Represents an accounting guide that is an aggregate of accounting rules.                       *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/

using System;
using System.Collections.Generic;

using Empiria.Json;
using Empiria.Parties;
using Empiria.StateEnums;

using Empiria.FinancialAccounting.AccountingRules.Data;

namespace Empiria.FinancialAccounting.AccountingRules {

  /// <summary>Represents an accounting guide that is an aggregate of accounting rules.</summary>
  public class AccountingGuide : BaseObject {

    #region Fields

    private Lazy<List<AccountingRule>> _accountingRules;

    #endregion Fields

    #region Constructors and parsers

    private AccountingGuide() {
      // Required by Empiria Framework.
    }

    static public AccountingGuide Parse(int id) => ParseId<AccountingGuide>(id);

    static public AccountingGuide Parse(string uid) => ParseKey<AccountingGuide>(uid);

    static public AccountingGuide ParseWithCode(string code) {
      var guide = TryParse<AccountingGuide>($"ACG_CODE = '{code}'");

      Assertion.Require(guide, $"There is no accounting guide with code '{code}'.");

      return guide;
    }


    static public AccountingGuide Empty => ParseEmpty<AccountingGuide>();


    static public FixedList<AccountingGuide> GetList() {
      return BaseObject.GetList<AccountingGuide>()
                       .ToFixedList();
    }

    protected override void OnLoad() {
      _accountingRules = new Lazy<List<AccountingRule>>(() => AccountingRulesData.GetAccountingRules(this));
    }

    #endregion Constructors and parsers

    #region Properties

    [DataField("ACG_CODE")]
    public string Code {
      get; private set;
    }


    [DataField("ACG_NAME")]
    public string Name {
      get; private set;
    }


    [DataField("ACG_DESCRIPTION")]
    public string Description {
      get;
      private set;
    }


    [DataField("ACG_SOURCE_SYSTEM_CODE")]
    public string SourceSystemCode {
      get; private set;
    }


    [DataField("ACG_OPERATION_TYPE_CODE")]
    public string OperationTypeCode {
      get; private set;
    }


    [DataField("ACG_VOUCHER_TYPE_CODE")]
    public string VoucherTypeCode {
      get; private set;
    }


    [DataField("ACG_RULES_EXT_DATA")]
    public JsonObject ExtData {
      get; private set;
    }


    [DataField("ACG_START_DATE")]
    public DateTime StartDate {
      get; private set;
    }


    [DataField("ACG_END_DATE")]
    public DateTime EndDate {
      get; private set;
    }

    [DataField("ACG_POSTED_BY_ID")]
    public Party PostedBy {
      get; private set;
    }


    [DataField("ACG_STATUS", Default = EntityStatus.Active)]
    public EntityStatus Status {
      get; private set;
    }


    public AccountingGuideAccounts Accounts {
      get {
        return new AccountingGuideAccounts(ExtData);
      }
    }


    public FixedList<AccountingRule> Rules {
      get {
        return _accountingRules.Value.ToFixedList();
      }
    }

    #endregion Properties

  }  // class AccountingGuide

}  // namespace Empiria.FinancialAccounting.AccountingRules
