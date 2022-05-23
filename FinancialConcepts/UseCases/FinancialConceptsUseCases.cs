/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Use case interactor class               *
*  Type     : FinancialConceptsUseCases                  License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases that handles financial concepts.                                                     *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.FinancialConcepts.Adapters;

namespace Empiria.FinancialAccounting.FinancialConcepts.UseCases {

  /// <summary>Use cases that handles financial concepts.</summary>
  public class FinancialConceptsUseCases : UseCase {

    #region Constructors and parsers

    protected FinancialConceptsUseCases() {
      // no-op
    }

    static public FinancialConceptsUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<FinancialConceptsUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases


    public void CleanupFinancialConceptGroup(string groupUID) {
      Assertion.AssertObject(groupUID, "groupUID");

      var group = FinancialConceptGroup.Parse(groupUID);

      group.Cleanup();
    }


    public FixedList<FinancialConceptIntegrationEntryDto> GetFinancialConceptIntegration(string financialConceptUID) {
      Assertion.AssertObject(financialConceptUID, nameof(financialConceptUID));

      var financialConcept = FinancialConcept.Parse(financialConceptUID);

      return FinancialConceptMapper.Map(financialConcept.Integration);
    }


    public FixedList<NamedEntityDto> GetFinancialConceptsGroups(string accountsChartUID) {
      Assertion.AssertObject(accountsChartUID, "accountsChartUID");

      var accountsChart = AccountsChart.Parse(accountsChartUID);

      FixedList<FinancialConceptGroup> groups = FinancialConceptGroup.GetList(accountsChart);

      return groups.MapToNamedEntityList();
    }


    public FixedList<FinancialConceptDto> GetGroupFinancialConcepts(string groupUID) {
      Assertion.AssertObject(groupUID, "groupUID");

      var group = FinancialConceptGroup.Parse(groupUID);

      FixedList<FinancialConcept> concepts = group.FinancialConcepts();

      return FinancialConceptMapper.Map(concepts);
    }


    public FixedList<FinancialConceptEntryAsTreeNodeDto> GetGroupFinancialConceptsEntriesAsTree(string groupUID) {
      Assertion.AssertObject(groupUID, "groupUID");

      var group = FinancialConceptGroup.Parse(groupUID);

      FinancialConceptEntriesTree tree = group.GetFinancialConceptsEntriesAsTree();

      return FinancialConceptsTreeMapper.MapFlat(tree.GetNodes());
    }


    #endregion Use cases

  }  // class FinancialConceptsUseCases

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.UseCases
