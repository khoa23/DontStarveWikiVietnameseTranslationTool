using DonStarveWikiTranslator.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DonStarveWikiTranslator.Forms
{
    public partial class MainForm : Form
    {
        private readonly WikiService _wikiService;
        private CancellationTokenSource _cts;

        public MainForm()
        {
            InitializeComponent();
            _wikiService = new WikiService();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Display database statistics on load
            UpdateDatabaseStatus();
        }

        private int GetSyncLimit()
        {
            if (int.TryParse(txtLimit.Text, out int limit) && limit > 0)
            {
                return limit;
            }
            return 10000; // Large enough for "All"
        }

        private void UpdateDatabaseStatus()
        {
            try
            {
                var stats = _wikiService.GetDatabaseStats();
                lblDbStatus.Text = $"Database: {stats.Total} articles | Missing: {stats.Missing} | Outdated: {stats.Outdated} | Up-to-date: {stats.UpToDate}";
                lblDbStatus.ForeColor = stats.Total > 0 ? Color.Green : Color.Gray;
            }
            catch (Exception ex)
            {
                lblDbStatus.Text = $"Database error: {ex.Message}";
                lblDbStatus.ForeColor = Color.Red;
                Logger.Log($"Error updating database status: {ex.Message}");
            }
        }

        private async void btnAnalyzeMissing_Click(object sender, EventArgs e)
        {
            Logger.Log("[MainForm] User clicked 'Analyze Missing Articles' button.");
            _cts = new CancellationTokenSource();
            try
            {
                btnAnalyzeMissing.Enabled = false;
                btnCancelSync.Visible = true;
                btnCancelSync.Enabled = true;
                lblStatus.Text = "Analyzing missing articles...";
                dgvMissing.Rows.Clear();

                var forceRefresh = chkForceRefresh.Checked;
                var limit = GetSyncLimit();
                var missingArticles = await _wikiService.GetMissingArticles(limit, forceRefresh, _cts.Token);

                foreach (var title in missingArticles)
                {
                    dgvMissing.Rows.Add(title);
                }

                lblStatus.Text = $"Found {missingArticles.Count} missing articles" + 
                    (forceRefresh ? " (from API)" : " (from cache)");
                
                UpdateDatabaseStatus();
            }
            catch (OperationCanceledException)
            {
                lblStatus.Text = "Operation cancelled.";
                Logger.Log("[MainForm] Operation cancelled by user.");
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error occurred";
                Logger.Log($"Error in Analyze Missing: {ex.ToString()}");
            }
            finally
            {
                btnAnalyzeMissing.Enabled = true;
                btnCancelSync.Visible = false;
                _cts?.Dispose();
                _cts = null;
            }
        }

        private async void btnAnalyzeOutdated_Click(object sender, EventArgs e)
        {
            Logger.Log("[MainForm] User clicked 'Analyze Outdated Articles' button.");
            _cts = new CancellationTokenSource();
            try
            {
                btnAnalyzeOutdated.Enabled = false;
                btnCancelSync.Visible = true;
                btnCancelSync.Enabled = true;
                lblStatus.Text = "Analyzing outdated articles...";
                dgvOutdated.Rows.Clear();

                var forceRefresh = chkForceRefresh.Checked;
                var limit = GetSyncLimit();
                var outdatedArticles = await _wikiService.GetOutdatedArticles(limit, forceRefresh, _cts.Token);

                foreach (var article in outdatedArticles)
                {
                    dgvOutdated.Rows.Add(
                        article.Title,
                        article.VietnameseTitle ?? "",
                        article.EnglishLastUpdate.ToString("yyyy-MM-dd HH:mm"),
                        article.VietnameseLastUpdate.ToString("yyyy-MM-dd HH:mm"),
                        article.DaysBehind
                    );
                }

                lblStatus.Text = $"Found {outdatedArticles.Count} outdated articles" +
                    (forceRefresh ? " (from API)" : " (from cache)");
                
                UpdateDatabaseStatus();
            }
            catch (OperationCanceledException)
            {
                lblStatus.Text = "Operation cancelled.";
                Logger.Log("[MainForm] Operation cancelled by user.");
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error occurred";
                Logger.Log($"Error in Analyze Outdated: {ex.ToString()}");
            }
            finally
            {
                btnAnalyzeOutdated.Enabled = true;
                btnCancelSync.Visible = false;
                _cts?.Dispose();
                _cts = null;
            }
        }

        private async void btnSyncDatabase_Click(object sender, EventArgs e)
        {
            Logger.Log("[MainForm] User clicked 'Sync Database' button.");
            _cts = new CancellationTokenSource();
            try
            {
                btnSyncDatabase.Enabled = false;
                btnCancelSync.Visible = true;
                btnCancelSync.Enabled = true;
                lblStatus.Text = "Syncing database with wiki API... (Downloading content)";

                // Force refresh both missing and outdated articles
                var originalForceRefresh = chkForceRefresh.Checked;
                chkForceRefresh.Checked = true;

                var limit = GetSyncLimit();
                await _wikiService.GetMissingArticles(limit, true, _cts.Token);
                await _wikiService.GetOutdatedArticles(limit, true, _cts.Token);

                chkForceRefresh.Checked = originalForceRefresh;

                UpdateDatabaseStatus();
                lblStatus.Text = "Database sync completed successfully!";

                MessageBox.Show("Database has been synchronized with the latest wiki data (including page content).", 
                    "Sync Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (OperationCanceledException)
            {
                lblStatus.Text = "Sync cancelled.";
                Logger.Log("[MainForm] Sync cancelled by user.");
                UpdateDatabaseStatus();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Sync error occurred";
                Logger.Log($"Error in Sync Database: {ex.ToString()}");
            }
            finally
            {
                btnSyncDatabase.Enabled = true;
                btnCancelSync.Visible = false;
                _cts?.Dispose();
                _cts = null;
            }
        }

        private void btnCancelSync_Click(object sender, EventArgs e)
        {
            Logger.Log("[MainForm] User clicked 'Cancel Sync' button.");
            _cts?.Cancel();
            btnCancelSync.Enabled = false;
            lblStatus.Text = "Cancelling...";
        }

        private void dgvMissing_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var title = dgvMissing.Rows[e.RowIndex].Cells[0].Value?.ToString();
                Logger.Log($"[MainForm] User double-clicked missing article: {title}");
                if (!string.IsNullOrEmpty(title))
                {
                    var article = _wikiService.GetArticleFromCache(title);
                    var url = article?.EnglishUrl ?? $"{AppConfig.EnglishWikiUrl}/{Uri.EscapeDataString(title)}";
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
            }
        }

        private void dgvOutdated_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var title = dgvOutdated.Rows[e.RowIndex].Cells[0].Value?.ToString();
                Logger.Log($"[MainForm] User double-clicked outdated article: {title}");
                if (!string.IsNullOrEmpty(title))
                {
                    var article = _wikiService.GetArticleFromCache(title);
                    var url = article?.EnglishUrl ?? $"{AppConfig.EnglishWikiUrl}/{Uri.EscapeDataString(title)}";
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _wikiService?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
