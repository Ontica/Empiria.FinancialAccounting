/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Accounting Rules                   Component : Domain Layer                          *
*  Assembly : FinancialAccounting.AccountingRules.dll      Pattern   : Information holder                    *
*  Type     : ClassificationGuide                          License   : Please read LICENSE.txt file          *
*                                                                                                            *
*  Summary  : Groups accounting classification rules.                                                        *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/


using System;
using System.Collections.Generic;

using Empiria.Json;
using Empiria.Parties;
using Empiria.StateEnums;

using Empiria.FinancialAccounting.AccountingRules.Data;

namespace Empiria.FinancialAccounting.AccountingRules {

  /// <summary>Groups accounting classification rules.</summary>
  public sealed class ClassificationGuide : BaseObject {

    private Lazy<List<ClassificationRule>> _rules;


    #region Constructors and parsers

    private ClassificationGuide() {
      // Required by Empiria Framework.       
    }

    static public ClassificationGuide Parse(int id) => ParseId<ClassificationGuide>(id);

    static public ClassificationGuide Parse(string uid) => ParseKey<ClassificationGuide>(uid);

    static public ClassificationGuide ParseWithCode(string code) {
      Assertion.Require(code, nameof(code));

      var guide = TryParse<ClassificationGuide>($"ACG_CODE = '{code}'");

      Assertion.Require(guide, $"There is no classification guide with code '{code}'.");

      return guide;
    }

    static public FixedList<ClassificationGuide> GetList() {
      return BaseObject.GetList<ClassificationGuide>().ToFixedList();
    }

    static public ClassificationGuide Empty => ParseEmpty<ClassificationGuide>();

    protected override void OnLoad() {
      _rules = new Lazy<List<ClassificationRule>>(() => ClassificationRulesData.GetRules(this));
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
      get; private set;
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


    public FixedList<ClassificationRule> Rules {
      get {
        return _rules.Value.ToFixedList();
      }
    }

    #endregion Properties

  }  // class ClassificationGuide

}  // namespace Empiria.FinancialAccounting.AccountingRules
