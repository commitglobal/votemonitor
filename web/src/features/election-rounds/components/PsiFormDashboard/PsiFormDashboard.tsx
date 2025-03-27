import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Separator } from '@/components/ui/separator';
import { useTranslation } from 'react-i18next';

export interface PsiFormDashboardProps {
  electionRoundId: string;
}
function PsiFormDashboard({ electionRoundId }: PsiFormDashboardProps) {
  const { t } = useTranslation();

  return (
    <Card>
      <CardHeader className='flex gap-2 flex-column'>
        <div className='flex flex-row items-center justify-between'>
          <CardTitle className='text-2xl font-semibold leading-none tracking-tight'>
            {t('electionEvent.psiForm.cardTitle')}
          </CardTitle>
        </div>
        <Separator />
      </CardHeader>
      <CardContent className='flex flex-col items-baseline gap-6'>{electionRoundId}</CardContent>
    </Card>
  );
}

export default PsiFormDashboard;
