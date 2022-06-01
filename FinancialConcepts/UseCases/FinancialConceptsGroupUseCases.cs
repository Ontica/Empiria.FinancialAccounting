﻿/* Empiria Financial *****************************************************************************************
*                                                                                                            *
*  Module   : Financial Concepts                         Component : Use cases Layer                         *
*  Assembly : FinancialAccounting.FinancialConcepts.dll  Pattern   : Use case interactor class               *
*  Type     : FinancialConceptsGroupUseCases             License   : Please read LICENSE.txt file            *
*                                                                                                            *
*  Summary  : Use cases that retrieves financial concepts and their integrations from groups.                *
*                                                                                                            *
************************* Copyright(c) La Vía Óntica SC, Ontica LLC and contributors. All rights reserved. **/
using System;

using Empiria.Services;

using Empiria.FinancialAccounting.FinancialConcepts.Adapters;

namespace Empiria.FinancialAccounting.FinancialConcepts.UseCases {

  /// <summary>Use cases that retrieves financial concepts and their integrations from groups.</summary>
  public class FinancialConceptsGroupUseCases : UseCase {

    #region Constructors and parsers

    protected FinancialConceptsGroupUseCases() {
      // no-op
    }

    static public FinancialConceptsGroupUseCases UseCaseInteractor() {
      return UseCase.CreateInstance<FinancialConceptsGroupUseCases>();
    }

    #endregion Constructors and parsers

    #region Use cases


    public void CleanupFinancialConceptGroup(string groupUID) {
      Assertion.Require(groupUID, "groupUID");

      var group = FinancialConceptGroup.Parse(groupUID);

      group.Cleanup();
    }


    public FixedList<FinancialConceptsGroupDto> GetFinancialConceptsGroups() {
      FixedList<FinancialConceptGroup> groups = FinancialConceptGroup.GetList();

      return FinancialConceptMapper.Map(groups);
    }


    public FixedList<FinancialConceptDescriptorDto> GetFinancialConceptsInGroup(string groupUID) {
      Assertion.Require(groupUID, "groupUID");

      var group = FinancialConceptGroup.Parse(groupUID);

      return FinancialConceptMapper.Map(group.FinancialConcepts);
    }


    public FixedList<FinancialConceptEntryAsTreeNodeDto> GetGroupIntegrationEntriesAsTree(string groupUID) {
      Assertion.Require(groupUID, "groupUID");

      var group = FinancialConceptGroup.Parse(groupUID);

      var tree = new FinancialConceptsEntriesTree(group);

      return FinancialConceptsTreeMapper.Map(tree.GetNodes());
    }


    #endregion Use cases

  }  // class FinancialConceptsGroupUseCases

}  // namespace Empiria.FinancialAccounting.FinancialConcepts.UseCases