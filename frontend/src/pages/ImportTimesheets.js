import { useState, useEffect } from 'react';
import axios from 'axios';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '../components/ui/card';
import { Button } from '../components/ui/button';
import { Badge } from '../components/ui/badge';
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from '../components/ui/table';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '../components/ui/select';
import {
  Alert,
  AlertDescription,
  AlertTitle,
} from '../components/ui/alert';
import { toast } from 'sonner';
import { formatDate } from '../lib/utils';
import { FileDown, AlertCircle, CheckCircle2, AlertTriangle, Loader2, Users } from 'lucide-react';

const API = `${process.env.REACT_APP_BACKEND_URL}/api`;

export default function ImportTimesheets() {
  const [periods, setPeriods] = useState([]);
  const [selectedPeriod, setSelectedPeriod] = useState('');
  const [preview, setPreview] = useState(null);
  const [loading, setLoading] = useState(true);
  const [previewing, setPreviewing] = useState(false);
  const [importing, setImporting] = useState(false);

  useEffect(() => {
    fetchPeriods();
  }, []);

  const fetchPeriods = async () => {
    try {
      const response = await axios.get(`${API}/pay-periods`);
      // Only show open periods
      setPeriods(response.data.filter(p => p.status === 'open'));
    } catch (error) {
      toast.error('Failed to fetch pay periods');
    } finally {
      setLoading(false);
    }
  };

  const handlePreview = async () => {
    if (!selectedPeriod) {
      toast.error('Please select a pay period');
      return;
    }
    
    setPreviewing(true);
    setPreview(null);
    try {
      const response = await axios.post(`${API}/timesheets/preview`, {
        pay_period_id: selectedPeriod
      });
      setPreview(response.data);
    } catch (error) {
      toast.error(error.response?.data?.detail || 'Failed to preview timesheets');
    } finally {
      setPreviewing(false);
    }
  };

  const handleImport = async () => {
    if (!selectedPeriod) {
      toast.error('Please select a pay period');
      return;
    }
    
    setImporting(true);
    try {
      const response = await axios.post(`${API}/timesheets/import`, {
        pay_period_id: selectedPeriod
      });
      toast.success(`Successfully imported ${response.data.imported_count} time entries`);
      setPreview(null);
      setSelectedPeriod('');
    } catch (error) {
      toast.error(error.response?.data?.detail || 'Failed to import timesheets');
    } finally {
      setImporting(false);
    }
  };

  // Group entries by employee
  const groupByEmployee = (entries) => {
    const grouped = {};
    entries.forEach(entry => {
      const key = entry.employee_key || entry.employee_email;
      if (!grouped[key]) {
        const employeeName = entry.employee_name || 
          (entry.matched_employee 
            ? `${entry.matched_employee.first_name || ''} ${entry.matched_employee.last_name || ''}`.trim()
            : entry.employee_email || entry.employee_key || 'Unknown');
        grouped[key] = {
          employee_name: employeeName,
          employee_key: entry.employee_key,
          employee_email: entry.employee_email,
          matched_employee: entry.matched_employee,
          total_regular: 0,
          total_overtime: 0,
          entries: []
        };
      }
      grouped[key].total_regular += entry.regular_hours || 0;
      grouped[key].total_overtime += entry.overtime_hours || 0;
      grouped[key].entries.push(entry);
    });
    return Object.values(grouped);
  };

  const selectedPeriodData = periods.find(p => p.id === selectedPeriod);

  return (
    <div className="space-y-6" data-testid="import-timesheets-page">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Import Timesheets</h1>
        <p className="text-muted-foreground">Fetch approved timesheet entries from your timesheet system</p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Select Pay Period</CardTitle>
          <CardDescription>Choose an open pay period to import timesheets for</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          {loading ? (
            <div className="flex items-center justify-center py-4">
              <Loader2 className="h-6 w-6 animate-spin text-muted-foreground" />
            </div>
          ) : periods.length === 0 ? (
            <Alert>
              <AlertCircle className="h-4 w-4" />
              <AlertTitle>No Open Pay Periods</AlertTitle>
              <AlertDescription>
                Create a pay period first before importing timesheets.
              </AlertDescription>
            </Alert>
          ) : (
            <>
              <div className="flex flex-col sm:flex-row gap-4">
                <div className="flex-1">
                  <Select value={selectedPeriod} onValueChange={setSelectedPeriod}>
                    <SelectTrigger data-testid="period-select">
                      <SelectValue placeholder="Select a pay period" />
                    </SelectTrigger>
                    <SelectContent>
                      {periods.map((period) => (
                        <SelectItem key={period.id} value={period.id}>
                          {formatDate(period.start_date)} - {formatDate(period.end_date)}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
                <Button 
                  onClick={handlePreview} 
                  disabled={!selectedPeriod || previewing}
                  variant="outline"
                  data-testid="preview-btn"
                >
                  {previewing && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
                  <FileDown className="mr-2 h-4 w-4" />
                  Preview Timesheets
                </Button>
              </div>

              {selectedPeriodData && (
                <div className="bg-muted/50 rounded-lg p-4 text-sm">
                  <p><span className="font-medium">Period:</span> {formatDate(selectedPeriodData.start_date)} to {formatDate(selectedPeriodData.end_date)}</p>
                  <p><span className="font-medium">Pay Date:</span> {formatDate(selectedPeriodData.pay_date)}</p>
                </div>
              )}
            </>
          )}
        </CardContent>
      </Card>

      {preview && (
        <>
          {/* Unmatched Employees Warning */}
          {preview.unmatched_entries.length > 0 && (
            <Alert variant="destructive">
              <AlertTriangle className="h-4 w-4" />
              <AlertTitle>Unmatched Employees Found</AlertTitle>
              <AlertDescription>
                {preview.unmatched_entries.length} time entries could not be matched to employees. 
                Please map these employees in the Employees page first.
              </AlertDescription>
            </Alert>
          )}

          {/* Matched Entries */}
          <Card>
            <CardHeader>
              <div className="flex items-center justify-between">
                <div>
                  <CardTitle className="flex items-center gap-2">
                    <CheckCircle2 className="h-5 w-5 text-green-600" />
                    Matched Entries
                  </CardTitle>
                  <CardDescription>
                    {preview.matched_entries.length} entries from {groupByEmployee(preview.matched_entries).length} employees ready to import
                  </CardDescription>
                </div>
                <Button 
                  onClick={handleImport} 
                  disabled={importing || preview.matched_entries.length === 0}
                  data-testid="import-btn"
                >
                  {importing && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
                  Import {preview.matched_entries.length} Entries
                </Button>
              </div>
            </CardHeader>
            <CardContent>
              {preview.matched_entries.length === 0 ? (
                <div className="text-center py-8 text-muted-foreground">
                  No matched entries to import
                </div>
              ) : (
                <div className="rounded-md border">
                  <Table className="data-table">
                    <TableHeader>
                      <TableRow>
                        <TableHead>Employee</TableHead>
                        <TableHead>External Key</TableHead>
                        <TableHead className="text-right">Regular Hours</TableHead>
                        <TableHead className="text-right">Overtime Hours</TableHead>
                        <TableHead className="text-right">Total Hours</TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {groupByEmployee(preview.matched_entries).map((group, idx) => (
                        <TableRow key={idx}>
                          <TableCell className="font-medium">{group.employee_name}</TableCell>
                          <TableCell>
                            <Badge variant="secondary" className="font-mono">
                              {group.employee_key}
                            </Badge>
                          </TableCell>
                          <TableCell className="text-right">{group.total_regular.toFixed(1)}</TableCell>
                          <TableCell className="text-right">{group.total_overtime.toFixed(1)}</TableCell>
                          <TableCell className="text-right font-medium">
                            {(group.total_regular + group.total_overtime).toFixed(1)}
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </div>
              )}
            </CardContent>
          </Card>

          {/* Unmatched Entries Detail */}
          {preview.unmatched_entries.length > 0 && (
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2 text-amber-600">
                  <Users className="h-5 w-5" />
                  Unmatched Entries
                </CardTitle>
                <CardDescription>
                  These employees need to be mapped in the Employees page
                </CardDescription>
              </CardHeader>
              <CardContent>
                <div className="rounded-md border">
                  <Table className="data-table">
                    <TableHeader>
                      <TableRow>
                        <TableHead>Employee Name</TableHead>
                        <TableHead>External Key</TableHead>
                        <TableHead>Email</TableHead>
                        <TableHead className="text-right">Total Hours</TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {groupByEmployee(preview.unmatched_entries).map((group, idx) => (
                        <TableRow key={idx}>
                          <TableCell className="font-medium">{group.employee_name}</TableCell>
                          <TableCell>
                            <Badge variant="outline" className="font-mono">
                              {group.employee_key}
                            </Badge>
                          </TableCell>
                          <TableCell>{group.employee_email}</TableCell>
                          <TableCell className="text-right">
                            {(group.total_regular + group.total_overtime).toFixed(1)}
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </div>
              </CardContent>
            </Card>
          )}
        </>
      )}
    </div>
  );
}
