import { useState, useEffect } from 'react';
import axios from 'axios';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '../components/ui/card';
import { Button } from '../components/ui/button';
import { Input } from '../components/ui/input';
import { Label } from '../components/ui/label';
import { Switch } from '../components/ui/switch';
import { Textarea } from '../components/ui/textarea';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '../components/ui/select';
import {
  Tabs,
  TabsContent,
  TabsList,
  TabsTrigger,
} from '../components/ui/tabs';
import { toast } from 'sonner';
import { Settings as SettingsIcon, Building2, Database, Sliders, Loader2, Sparkles } from 'lucide-react';

const API = `${process.env.REACT_APP_BACKEND_URL}/api`;

export default function Settings() {
  const [company, setCompany] = useState(null);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [generatingDemo, setGeneratingDemo] = useState(false);
  const [settingsJson, setSettingsJson] = useState('');

  useEffect(() => {
    fetchCompany();
  }, []);

  const fetchCompany = async () => {
    try {
      const response = await axios.get(`${API}/company`);
      setCompany(response.data);
      setSettingsJson(JSON.stringify(response.data.settings_json, null, 2));
    } catch (error) {
      toast.error('Failed to fetch company settings');
    } finally {
      setLoading(false);
    }
  };

  const handleUpdateCompany = async (data) => {
    setSaving(true);
    try {
      await axios.put(`${API}/company`, data);
      toast.success('Company settings updated');
      fetchCompany();
    } catch (error) {
      toast.error(error.response?.data?.detail || 'Failed to update settings');
    } finally {
      setSaving(false);
    }
  };

  const handleUpdateTimesheetConfig = async (data) => {
    setSaving(true);
    try {
      await axios.put(`${API}/company/timesheet-config`, data);
      toast.success('Timesheet configuration updated');
      fetchCompany();
    } catch (error) {
      toast.error(error.response?.data?.detail || 'Failed to update configuration');
    } finally {
      setSaving(false);
    }
  };

  const handleSaveRates = async () => {
    try {
      const parsed = JSON.parse(settingsJson);
      await handleUpdateCompany({ settings_json: parsed });
    } catch (error) {
      toast.error('Invalid JSON format. Please check your input.');
    }
  };

  const handleGenerateDemo = async () => {
    setGeneratingDemo(true);
    try {
      const response = await axios.post(`${API}/demo/generate`);
      toast.success(`Generated ${response.data.employees_created} demo employees`);
    } catch (error) {
      toast.error(error.response?.data?.detail || 'Failed to generate demo data');
    } finally {
      setGeneratingDemo(false);
    }
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center py-16">
        <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
      </div>
    );
  }

  return (
    <div className="space-y-6" data-testid="settings-page">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Settings</h1>
        <p className="text-muted-foreground">Configure your company and payroll settings</p>
      </div>

      <Tabs defaultValue="company" className="space-y-6">
        <TabsList>
          <TabsTrigger value="company" data-testid="company-tab">
            <Building2 className="mr-2 h-4 w-4" />
            Company
          </TabsTrigger>
          <TabsTrigger value="timesheet" data-testid="timesheet-tab">
            <Database className="mr-2 h-4 w-4" />
            Timesheet API
          </TabsTrigger>
          <TabsTrigger value="rates" data-testid="rates-tab">
            <Sliders className="mr-2 h-4 w-4" />
            Rates & Brackets
          </TabsTrigger>
        </TabsList>

        {/* Company Settings */}
        <TabsContent value="company">
          <Card>
            <CardHeader>
              <CardTitle>Company Information</CardTitle>
              <CardDescription>Basic company details and payroll defaults</CardDescription>
            </CardHeader>
            <CardContent className="space-y-6">
              <div className="grid gap-4 md:grid-cols-2">
                <div className="space-y-2">
                  <Label htmlFor="company-name">Company Name</Label>
                  <Input
                    id="company-name"
                    value={company?.name || ''}
                    onChange={(e) => setCompany({ ...company, name: e.target.value })}
                    data-testid="company-name-input"
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="province">Province</Label>
                  <Select 
                    value={company?.province || 'ON'}
                    onValueChange={(value) => setCompany({ ...company, province: value })}
                  >
                    <SelectTrigger data-testid="province-select">
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="ON">Ontario</SelectItem>
                      <SelectItem value="BC" disabled>British Columbia (Coming Soon)</SelectItem>
                      <SelectItem value="AB" disabled>Alberta (Coming Soon)</SelectItem>
                      <SelectItem value="QC" disabled>Quebec (Coming Soon)</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
                <div className="space-y-2">
                  <Label htmlFor="pay-frequency">Pay Frequency</Label>
                  <Select 
                    value={company?.pay_frequency || 'biweekly'}
                    onValueChange={(value) => setCompany({ ...company, pay_frequency: value })}
                  >
                    <SelectTrigger data-testid="frequency-select">
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="weekly">Weekly (52 periods)</SelectItem>
                      <SelectItem value="biweekly">Bi-weekly (26 periods)</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
                <div className="space-y-2">
                  <Label htmlFor="default-rate">Default Hourly Rate (CAD)</Label>
                  <Input
                    id="default-rate"
                    type="number"
                    step="0.01"
                    value={company?.default_hourly_rate || 20}
                    onChange={(e) => setCompany({ ...company, default_hourly_rate: parseFloat(e.target.value) })}
                    data-testid="default-rate-input"
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="vacation-percent">Vacation Pay %</Label>
                  <Input
                    id="vacation-percent"
                    type="number"
                    step="0.1"
                    value={company?.vacation_pay_percent_default || 4}
                    onChange={(e) => setCompany({ ...company, vacation_pay_percent_default: parseFloat(e.target.value) })}
                    data-testid="vacation-percent-input"
                  />
                </div>
              </div>
              <Button 
                onClick={() => handleUpdateCompany({
                  name: company.name,
                  province: company.province,
                  pay_frequency: company.pay_frequency,
                  default_hourly_rate: company.default_hourly_rate,
                  vacation_pay_percent_default: company.vacation_pay_percent_default
                })} 
                disabled={saving}
                data-testid="save-company-btn"
              >
                {saving && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
                Save Changes
              </Button>
            </CardContent>
          </Card>

          {/* Demo Data Card */}
          <Card className="mt-6">
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Sparkles className="h-5 w-5 text-amber-500" />
                Demo Data
              </CardTitle>
              <CardDescription>Generate sample employees for testing</CardDescription>
            </CardHeader>
            <CardContent>
              <p className="text-sm text-muted-foreground mb-4">
                This will create 8 demo employees with various pay types (hourly and salary) 
                and pre-configured external employee keys for timesheet matching.
              </p>
              <Button 
                onClick={handleGenerateDemo} 
                disabled={generatingDemo}
                variant="outline"
                data-testid="generate-demo-btn"
              >
                {generatingDemo && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
                <Sparkles className="mr-2 h-4 w-4" />
                Generate Demo Employees
              </Button>
            </CardContent>
          </Card>
        </TabsContent>

        {/* Timesheet API Settings */}
        <TabsContent value="timesheet">
          <Card>
            <CardHeader>
              <CardTitle>Timesheet API Configuration</CardTitle>
              <CardDescription>Configure connection to your external timesheet system</CardDescription>
            </CardHeader>
            <CardContent className="space-y-6">
              <div className="flex items-center justify-between p-4 rounded-lg bg-muted/50">
                <div>
                  <Label htmlFor="use-mock" className="text-base">Use Mock Mode</Label>
                  <p className="text-sm text-muted-foreground">
                    Generate realistic test data instead of calling external API
                  </p>
                </div>
                <Switch
                  id="use-mock"
                  checked={company?.timesheet_config?.use_mock ?? true}
                  onCheckedChange={(checked) => handleUpdateTimesheetConfig({ use_mock: checked })}
                  data-testid="mock-toggle"
                />
              </div>

              <div className="grid gap-4">
                <div className="space-y-2">
                  <Label htmlFor="api-url">API Base URL</Label>
                  <Input
                    id="api-url"
                    value={company?.timesheet_config?.api_base_url || ''}
                    onChange={(e) => setCompany({
                      ...company,
                      timesheet_config: { ...company.timesheet_config, api_base_url: e.target.value }
                    })}
                    placeholder="https://your-timesheet-domain.com"
                    data-testid="api-url-input"
                  />
                </div>
                <div className="space-y-2">
                  <Label htmlFor="auth-method">Authentication Method</Label>
                  <Select 
                    value={company?.timesheet_config?.auth_method || 'API_KEY'}
                    onValueChange={(value) => setCompany({
                      ...company,
                      timesheet_config: { ...company.timesheet_config, auth_method: value }
                    })}
                  >
                    <SelectTrigger data-testid="auth-method-select">
                      <SelectValue />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="API_KEY">API Key</SelectItem>
                      <SelectItem value="BEARER">Bearer Token</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
                {company?.timesheet_config?.auth_method === 'API_KEY' && (
                  <>
                    <div className="space-y-2">
                      <Label htmlFor="api-key-header">API Key Header Name</Label>
                      <Input
                        id="api-key-header"
                        value={company?.timesheet_config?.api_key_header_name || 'X-API-KEY'}
                        onChange={(e) => setCompany({
                          ...company,
                          timesheet_config: { ...company.timesheet_config, api_key_header_name: e.target.value }
                        })}
                        data-testid="api-key-header-input"
                      />
                    </div>
                    <div className="space-y-2">
                      <Label htmlFor="api-key-value">API Key Value</Label>
                      <Input
                        id="api-key-value"
                        type="password"
                        value={company?.timesheet_config?.api_key_value || ''}
                        onChange={(e) => setCompany({
                          ...company,
                          timesheet_config: { ...company.timesheet_config, api_key_value: e.target.value }
                        })}
                        placeholder="Enter your API key"
                        data-testid="api-key-value-input"
                      />
                    </div>
                  </>
                )}
                {company?.timesheet_config?.auth_method === 'BEARER' && (
                  <div className="space-y-2">
                    <Label htmlFor="bearer-token">Bearer Token</Label>
                    <Input
                      id="bearer-token"
                      type="password"
                      value={company?.timesheet_config?.bearer_token || ''}
                      onChange={(e) => setCompany({
                        ...company,
                        timesheet_config: { ...company.timesheet_config, bearer_token: e.target.value }
                      })}
                      placeholder="Enter your bearer token"
                      data-testid="bearer-token-input"
                    />
                  </div>
                )}
              </div>
              <Button 
                onClick={() => handleUpdateTimesheetConfig(company.timesheet_config)} 
                disabled={saving}
                data-testid="save-timesheet-config-btn"
              >
                {saving && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
                Save Configuration
              </Button>
            </CardContent>
          </Card>
        </TabsContent>

        {/* Rates & Brackets */}
        <TabsContent value="rates">
          <Card>
            <CardHeader>
              <CardTitle>Rates & Tax Brackets</CardTitle>
              <CardDescription>
                Configure CPP, EI rates and tax brackets. Edit the JSON below carefully.
              </CardDescription>
            </CardHeader>
            <CardContent className="space-y-4">
              <div className="space-y-2">
                <Label htmlFor="settings-json">Settings JSON</Label>
                <Textarea
                  id="settings-json"
                  value={settingsJson}
                  onChange={(e) => setSettingsJson(e.target.value)}
                  className="font-mono text-sm min-h-[400px]"
                  data-testid="settings-json-textarea"
                />
                <p className="text-xs text-muted-foreground">
                  Includes CPP rate/max, EI rate/max, Federal tax brackets, and Ontario tax brackets.
                </p>
              </div>
              <Button 
                onClick={handleSaveRates} 
                disabled={saving}
                data-testid="save-rates-btn"
              >
                {saving && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
                Save Rates & Brackets
              </Button>
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>
    </div>
  );
}
