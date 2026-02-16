'use client'

import { useState } from 'react'
import { Dialog, DialogContent, DialogHeader, DialogTitle } from '@/components/ui/dialog'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { api } from '@/lib/api'

interface ScheduleMatchDialogProps {
    matchId: string
    currentKickoffTime?: string
    currentVenue?: string
    open: boolean
    onOpenChange: (open: boolean) => void
    onSuccess: () => void
}

export function ScheduleMatchDialog({
    matchId,
    currentKickoffTime,
    currentVenue,
    open,
    onOpenChange,
    onSuccess
}: ScheduleMatchDialogProps) {
    const [kickoffTime, setKickoffTime] = useState(
        currentKickoffTime ? new Date(currentKickoffTime).toISOString().slice(0, 16) : ''
    )
    const [venue, setVenue] = useState(currentVenue || '')
    const [loading, setLoading] = useState(false)

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault()
        setLoading(true)

        try {
            const response = await api.patch<any>(`/match/${matchId}/schedule`, {
                kickoffTime: kickoffTime ? new Date(kickoffTime).toISOString() : undefined,
                venue: venue || undefined
            })

            if (response.isSuccess) {
                onSuccess()
                onOpenChange(false)
            }
        } catch (error) {
            console.error('Error scheduling match:', error)
        } finally {
            setLoading(false)
        }
    }

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent className="bg-card border-border">
                <DialogHeader>
                    <DialogTitle>Schedule Match</DialogTitle>
                </DialogHeader>
                <form onSubmit={handleSubmit} className="space-y-4">
                    <div className="space-y-2">
                        <Label htmlFor="kickoffTime">Kickoff Time</Label>
                        <Input
                            id="kickoffTime"
                            type="datetime-local"
                            value={kickoffTime}
                            onChange={(e) => setKickoffTime(e.target.value)}
                            className="bg-background border-border"
                        />
                    </div>
                    <div className="space-y-2">
                        <Label htmlFor="venue">Venue</Label>
                        <Input
                            id="venue"
                            type="text"
                            placeholder="Enter venue name"
                            value={venue}
                            onChange={(e) => setVenue(e.target.value)}
                            className="bg-background border-border"
                        />
                    </div>
                    <div className="flex justify-end gap-2">
                        <Button
                            type="button"
                            variant="outline"
                            onClick={() => onOpenChange(false)}
                            disabled={loading}
                        >
                            Cancel
                        </Button>
                        <Button type="submit" disabled={loading}>
                            {loading ? 'Scheduling...' : 'Schedule Match'}
                        </Button>
                    </div>
                </form>
            </DialogContent>
        </Dialog>
    )
}
