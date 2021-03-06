﻿using System;
using System.Windows.Forms;
using RolePlayCharacterWF.ViewModels;
using RolePlayCharacter;

namespace RolePlayCharacterWF
{
    public partial class AddOrEditBeliefForm : Form
    {
        private KnowledgeBaseVM _knowledgeBaseVm;
        private BeliefDTO _beliefToEdit;

        public AddOrEditBeliefForm(KnowledgeBaseVM kbVM, BeliefDTO beliefToEdit = null)
        {
            InitializeComponent();
            
            _knowledgeBaseVm = kbVM;
            _beliefToEdit = beliefToEdit;

            perspectiveTextBox.Text = "SELF"; //Default Value
            certaintyTextBox.Text = "1";

            if (beliefToEdit != null)
            {
                this.Text = "Edit Belief";
                this.addOrEditBeliefButton.Text = "Update";

                beliefNameTextBox.Text = beliefToEdit.Name;
                beliefValueTextBox.Text = beliefToEdit.Value;
                perspectiveTextBox.Text = beliefToEdit.Perspective;
                certaintyTextBox.Text = certaintyTextBox.Text;
            }
        }

        private void addOrEditBeliefButton_Click(object sender, EventArgs e)
        {
            //clear errors
            addBeliefErrorProvider.Clear();

            try
            {
                var newBelief = new BeliefDTO
                {
                    Name = this.beliefNameTextBox.Text.Trim(),
                    Perspective = this.perspectiveTextBox.Text.Trim(),
                    Value = this.beliefValueTextBox.Text.Trim(),
                    Certainty = float.Parse(this.certaintyTextBox.Text.Trim())
                };
                if (_beliefToEdit != null)
                {
                    _knowledgeBaseVm.RemoveBeliefs(new[] {_beliefToEdit});
                    _knowledgeBaseVm.AddBelief(newBelief);
                    this.Close();
                }
                else
                {
                    _knowledgeBaseVm.AddBelief(newBelief);
                }
                Close();
            }
            catch (Exception ex)
            {
                addBeliefErrorProvider.SetError(beliefNameTextBox, ex.Message);
                if (_beliefToEdit != null)
                {
                    _knowledgeBaseVm.AddBelief(_beliefToEdit);
                }
                return;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void beliefVisibilityComboBox_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void beliefNameTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void beliefValueTextBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click_1(object sender, EventArgs e)
        {

        }

        private void label2_Click_1(object sender, EventArgs e)
        {

        }

        private void AddOrEditBeliefForm_Load(object sender, EventArgs e)
        {

        }
    }
}
