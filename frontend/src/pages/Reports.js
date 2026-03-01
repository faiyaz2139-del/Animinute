import { useState, useEffect } from 'react';
import axios from 'axios';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '../components/ui/card';
import { Button } from '../components/ui/button';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '../components/ui/select';
import { toast } from 'sonner';
import { formatDate } from '../lib/utils';
import { FileSpreadsheet, Download, Loader2 } from 'lucide-react';

const API = `${process.env.REACT_APP_BACKEND_URL}/api`;

export default function Reports() {
  const [runs, setRuns] = useState([]);
  const [selectedRun, setSelectedRun] = useState('');
  const [loading, setLoading] = useState(true);
  const [downloadingPayroll, setDownloadingPayroll] = useState(false);
  const [downloadingDeductions, setDownloadingDeductions] = useState(false);

  useEffect(() => {
    fetchRuns();
  }, []);

  const fetchRuns = async () => {
    try {
      const response = await axios.get(`${API}/payroll-runs`);
      // Only show locked runs for reports
      setRuns(response.data.filter(r => r.status === 'locked'));
    } catch (error) {
      toast.error('Failed to fetch payroll runs');
    } finally {
      setLoading(false);
    }
  };

  const downloadCSV = async (type) => {
    if (!selectedRun) {
      toast.error('Please select a payroll run');
      return;
    }

    const setDownloading = type === 'payroll' ? setDownloadingPayroll : setDownloadingDeductions;
    setDownloading(true);
    
    try {
      const endpoint = type === 'payroll' ? 'payroll-summary' : 'deductions-summary';
      const response = await axios.get(`${API}/reports/${endpoint}/${selectedRun}`, {
        responseType: 'blob'
      });
      
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', `${endpoint}_${selectedRun}.csv`);
      document.body.appendChild(link);
      link.click();
      link.remove();
      window.URL.revokeObjectURL(url);
      
      toast.success('Report downloaded');
    } catch (error) {
      toast.error('Failed to download report');
    } finally {
      setDownloading(false);
    }
  };

  return (
    <div className="space-y-6" data-testid="reports-page">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Reports</h1>
        <p className="text-muted-foreground">Export payroll data as CSV files</p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Select Payroll Run</CardTitle>
          <CardDescription>Choose a locked payroll run to generate reports</CardDescription>
        </CardHeader>
        <CardContent>
          {loading ? (
            <div className="flex items-center justify-center py-4">
              <Loader2 className="h-6 w-6 animate-spin text-muted-foreground" />
            </div>
          ) : runs.length === 0 ? (
            <p className="text-muted-foreground">No locked payroll runs available. Lock a payroll run first to generate reports.</p>
          ) : (
            <div className="max-w-sm">
              <Select value={selectedRun} onValueChange={setSelectedRun}>
                <SelectTrigger data-testid="run-select">
                  <SelectValue placeholder="Select a payroll run" />
                </SelectTrigger>
                <SelectContent>
                  {runs.map((run) => (
                    <SelectItem key={run.id} value={run.id}>
                      Run #{run.run_number} - {formatDate(run.locked_at)}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          )}
        </CardContent>
      </Card>

      <div className="grid gap-6 md:grid-cols-2">
        <Card className="card-hover">
          <CardHeader>
            <div className="flex items-center gap-3">
              <div className="h-10 w-10 rounded-lg bg-primary/10 flex items-center justify-center">
                <FileSpreadsheet className="h-5 w-5 text-primary" />
              </div>
              <div>
                <CardTitle className="text-lg">Payroll Summary</CardTitle>
                <CardDescription>Complete payroll breakdown per employee</CardDescription>
              </div>
            </div>
          </CardHeader>
          <CardContent>
            <p className="text-sm text-muted-foreground mb-4">
              Includes: Employee details, hours worked, gross pay, all deductions, and net pay.
            </p>
            <Button 
              onClick={() => downloadCSV('payroll')} 
              disabled={!selectedRun || downloadingPayroll}
              className="w-full btn-scale"
              data-testid="download-payroll-csv"
            >
              {downloadingPayroll && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
              <Download className="mr-2 h-4 w-4" />
              Download Payroll CSV
            </Button>
          </CardContent>
        </Card>

        <Card className="card-hover">
          <CardHeader>
            <div className="flex items-center gap-3">
              <div className="h-10 w-10 rounded-lg bg-secondary/20 flex items-center justify-center">
                <FileSpreadsheet className="h-5 w-5 text-secondary" />
              </div>
              <div>
                <CardTitle className="text-lg">Deductions Summary</CardTitle>
                <CardDescription>Breakdown of all payroll deductions</CardDescription>
              </div>
            </div>
          </CardHeader>
          <CardContent>
            <p className="text-sm text-muted-foreground mb-4">
              Includes: CPP, EI, Federal Tax, Provincial Tax, and totals per employee.
            </p>
            <Button 
              onClick={() => downloadCSV('deductions')} 
              disabled={!selectedRun || downloadingDeductions}
              variant="outline"
              className="w-full btn-scale"
              data-testid="download-deductions-csv"
            >
              {downloadingDeductions && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
              <Download className="mr-2 h-4 w-4" />
              Download Deductions CSV
            </Button>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
