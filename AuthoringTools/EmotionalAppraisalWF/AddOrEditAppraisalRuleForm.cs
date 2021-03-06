﻿using AutobiographicMemory;
using Conditions.DTOs;
using EmotionalAppraisal.DTOs;
using EmotionalAppraisalWF.Properties;
using EmotionalAppraisalWF.ViewModels;
using System;
using System.Windows.Forms;
using WellFormedNames;

namespace EmotionalAppraisalWF
{
    public partial class AddOrEditAppraisalRuleForm : Form
    {
        private AppraisalRulesVM _appraisalRulesVM;
        private AppraisalRuleDTO _appraisalRuleToEdit;

        public AddOrEditAppraisalRuleForm(AppraisalRulesVM ruleVM, AppraisalRuleDTO ruleToEdit = null)
        {
            InitializeComponent();

            _appraisalRulesVM = ruleVM;
            _appraisalRuleToEdit = ruleToEdit;

            //defaultValues
            comboBoxDesirability.Text = "0";
            comboBoxPraiseworthiness.Text = "0";
            comboBoxEventType.DataSource = AppraisalRulesVM.EventTypes;

            if (ruleToEdit != null)
            {
                this.Text = Resources.EditAppraisalRuleFormTitle;
                this.addOrEditButton.Text = Resources.UpdateButtonLabel;
                comboBoxEventType.Text = ruleToEdit.EventMatchingTemplate.GetNTerm(1).ToString();
                textBoxSubject.Text = ruleToEdit.EventMatchingTemplate.GetNTerm(2).ToString();
                textBoxObject.Text = ruleToEdit.EventMatchingTemplate.GetNTerm(3).ToString();
                textBoxTarget.Text = ruleToEdit.EventMatchingTemplate.GetNTerm(4).ToString();
                comboBoxDesirability.Text = ruleToEdit.Desirability.ToString();
                comboBoxPraiseworthiness.Text = ruleToEdit.Praiseworthiness.ToString();
            }
        }

        private void addOrEditButton_Click_1(object sender, EventArgs e)
        {
            AppraisalRuleDTO newRule = new AppraisalRuleDTO();
                        
            newRule = new AppraisalRuleDTO()
            {
                EventMatchingTemplate = WellFormedNames.Name.BuildName(
                    (Name)AMConsts.EVENT, 
                    (Name)comboBoxEventType.Text,
                    (Name)textBoxSubject.Text,
                    (Name)textBoxObject.Text,
                    (Name)textBoxTarget.Text),
                Desirability = WellFormedNames.Name.BuildName(Int32.Parse(comboBoxDesirability.Text)),
                Praiseworthiness = WellFormedNames.Name.BuildName(Int32.Parse(comboBoxPraiseworthiness.Text)),
                Conditions = new ConditionSetDTO()
            };
            try
            {
                if (_appraisalRuleToEdit != null)
                {
                    newRule.Id = _appraisalRuleToEdit.Id;
                    newRule.Conditions = _appraisalRuleToEdit.Conditions;
                }
                _appraisalRulesVM.AddOrUpdateAppraisalRule(newRule);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Resources.ErrorDialogTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBoxEventType_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void textBoxSubject_TextChanged(object sender, EventArgs e)
        {
        }

        private void label4_Click(object sender, EventArgs e)
        {
        }

        private void labelObject_Click(object sender, EventArgs e)
        {

        }
    }
}