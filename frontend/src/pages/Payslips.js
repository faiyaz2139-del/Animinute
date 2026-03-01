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
import { toast } from 'sonner';
import { formatDate } from '../lib/utils';
import { useAuth } from '../context/AuthContext';
import { FileText, Download, Loader2 } from 'lucide-react';

const API = `${process.env.REACT_APP_BACKEND_URL}/api`;

export default function Payslips() {
  const { user } = useAuth();
  const [payslips, setPayslips] = useState([]);
  const [loading, setLoading] = useState(true);
  const [downloading, setDownloading] = useState({});

  useEffect(() => {
    fetchPayslips();
  }, []);

  const fetchPayslips = async () => {
    try {
      const response = await axios.get(`${API}/payslips`);
      setPayslips(response.data);
    } catch (error) {
      toast.error('Failed to fetch payslips');
    } finally {
      setLoading(false);
    }
  };

  const handleDownload = async (payslipId, employeeName) => {
    setDownloading(prev => ({ ...prev, [payslipId]: true }));
    try {
      const response = await axios.get(`${API}/payslips/${payslipId}/pdf`, {
        responseType: 'blob'
      });
      
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', `payslip_${employeeName.replace(/\s+/g, '_')}.pdf`);
      document.body.appendChild(link);
      link.click();
      link.remove();
      window.URL.revokeObjectURL(url);
      
      toast.success('Payslip downloaded');
    } catch (error) {
      toast.error('Failed to download payslip');
    } finally {
      setDownloading(prev => ({ ...prev, [payslipId]: false }));
    }
  };

  const isAdmin = user?.role === 'admin' || user?.role === 'manager';

  return (
    <div className="space-y-6" data-testid="payslips-page">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Payslips</h1>
        <p className="text-muted-foreground">
          {isAdmin ? 'View and download all employee payslips' : 'View and download your payslips'}
        </p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <FileText className="h-5 w-5" />
            {isAdmin ? 'All Payslips' : 'My Payslips'}
          </CardTitle>
          <CardDescription>
            {isAdmin ? 'Download payslips for any employee' : 'Download your payment statements'}
          </CardDescription>
        </CardHeader>
        <CardContent>
          {loading ? (
            <div className="flex items-center justify-center py-8">
              <Loader2 className="h-8 w-8 animate-spin text-muted-foreground" />
            </div>
          ) : payslips.length === 0 ? (
            <div className="text-center py-8 text-muted-foreground">
              No payslips available yet. Payslips are generated after a payroll run is locked.
            </div>
          ) : (
            <div className="rounded-md border">
              <Table className="data-table">
                <TableHeader>
                  <TableRow>
                    {isAdmin && <TableHead>Employee</TableHead>}
                    <TableHead>Created</TableHead>
                    <TableHead className="text-right">Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {payslips.map((payslip) => (
                    <TableRow key={payslip.id} data-testid={`payslip-row-${payslip.id}`}>
                      {isAdmin && (
                        <TableCell className="font-medium">{payslip.employee_name}</TableCell>
                      )}
                      <TableCell>{formatDate(payslip.created_at)}</TableCell>
                      <TableCell className="text-right">
                        <Button
                          variant="outline"
                          size="sm"
                          onClick={() => handleDownload(payslip.id, payslip.employee_name)}
                          disabled={downloading[payslip.id]}
                          data-testid={`download-payslip-${payslip.id}`}
                        >
                          {downloading[payslip.id] ? (
                            <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                          ) : (
                            <Download className="mr-2 h-4 w-4" />
                          )}
                          Download PDF
                        </Button>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
